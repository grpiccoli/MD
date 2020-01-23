'use strict';
const puppeteer = require('puppeteer');
const anticaptchaAsync = require('anticaptcha-async');
const fs = require('fs');
const path = require('path');
const querystring = require('querystring');

const png = 'tmp.png';
const file_path = path.join(__dirname, png);
const selector = '#captcha';
let cllbck, brw;

const end = async (msg, success = false) => {
    brw.close().then(_ => {
        if (success) {
            cllbck(null, msg);
        } else {
            throw msg;
        }
    }).catch(_ => {
        if (success) {
            cllbck(null, msg);
        } else {
            throw msg;
        }
    });
};

const screenshotDOMElement = async (page, opts) => {
    const padding = 'padding' in opts ? opts.padding : 0;
    const imgPath = 'imgPath' in opts ? opts.imgPath : null;
    const selector = opts.selector;

    if (!selector)
        end('Please provide a selector.');

    const rect = await page.evaluate(selector => {
        const element = document.querySelector(selector);
        if (!element)
            return null;
        const drect = element.getBoundingClientRect();
        return {
            left: drect.x,
            top: drect.y,
            width: drect.width,
            height: drect.height
        };
    }, selector).catch(e => end(25 + e));

    if (!rect)
        end(`Could not find element that matches selector: ${selector}.`);

    return await page.screenshot({
        path: imgPath,
        clip: {
            x: rect.left - padding,
            y: rect.top - padding,
            width: rect.width + padding * 2,
            height: rect.height + padding * 2
        }
    }).catch(e => end(38 + e));
};

const readCaptcha = async acKey => {
    const page = (await brw.pages())[0];
    await screenshotDOMElement(page, {
        imgPath: file_path,
        selector: selector
    }).catch(e => end(45 + e));
    const client = anticaptchaAsync(acKey);
    const result = await client.getImage(fs.createReadStream(file_path))
        .catch(e => end(48 + e));
    return result.getValue();
};

const type = async (page, selector, value) => {
    await page.evaluate(selector => {
        document.querySelector(selector).value = '';
    }, selector);
    await page.type(selector, value);
};

const submitCatpcha = async (page, rut, captcha) => {
    await type(page, '#RutBeneficiario', rut);
    await type(page, '#captcha_code', captcha);
    await page.click('#btnCertifBenef');
};

const initBrowser = async () => {
    await puppeteer.launch(
        {
            ignoreHTTPSErrors: true,
            headless: true,
            args: ['--no-sandbox', '--disable-setuid-sandbox']
        }
    ).then(async browser => {
        brw = browser;
        const page = (await brw.pages())[0];
        page.setViewport({ width: 1000, height: 600, deviceScaleFactor: 1 });
        await page.goto('https://bonowebfon.fonasa.cl/', { waitUntil: 'networkidle2' });
    }).catch(e => end(e));
};

//data:
//rut, phone, email, docRut, specialty, region, commune, payRut, acKey
module.exports = async (callback, data) => {
    cllbck = callback;
    console.log(data);
    await initBrowser()
        .then(async () => {
            await readCaptcha(data.acKey)
                .then(async captcha => {
                    const page = (await brw.pages())[0];
                    page.on('response', response => {
                        let url = response.url();
                        let params = querystring.decode(url.split('?')[1]);
                        response.text()
                            .then(async textBody => {
                            switch (params.action) {
                                case 'execWSCertifTrab':
                                    if (textBody === 'ERROR_CAPTCHA') {
                                        await page.reload({ waitUntil: ['networkidle0', 'domcontentloaded'] });
                                        captcha = await readCaptcha(page, data.acKey)
                                            .catch(e => end(e));
                                        await submitCatpcha(page, data.rut, captcha)
                                            .catch(e => end(e));
                                        break;
                                    }
                                    var dest = path.resolve(__dirname, 'trainset', `${captcha}.png`);
                                    await fs.rename(file_path, dest, function (err) {
                                        if (err) end(err);
                                    });
                                    await type(page, '#CelularNotificacion', data.phone);
                                    await type(page, '#RepiteCelular', data.phone);
                                    await type(page, '#EmailNotificacion', data.email);
                                    await page.select('#TipoBusquedaProfesional', 'RutProfesional');
                                    await type(page, '#PalabraClave', data.docRut);
                                    //await page.select('#Especialidad', data.specialty);
                                    await page.select('#Region', data.region);
                                    break;
                                case 'BwPrmComunaS':
                                    await page.select('#Comuna', data.commune);
                                    await page.click('#btnBuscaProfesional');
                                    break;
                                case 'BuscaPorProfesional':
                                    await page.click('.btnSeleccionaAtencion:first-of-type');
                                    break;
                                case 'validaCAT':
                                    const newTarget = await browser
                                        .waitForTarget(target => target.url()
                                            .startsWith('https://bonowebfon.fonasa.cl/index.php?controller=bono&action=pago'));
                                    const newPage = await newTarget.page();
                                    await newPage.type('#RutPagador', data.payRut);
                                    const json = await newPage.evaluate(async rut => {
                                        var url = urlAjax('bono', 'execWSCertifPagador');
                                        return await fetch(url, {
                                            method: 'POST',
                                            headers: new Headers({
                                                'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8'
                                            }),
                                            body: `RutPagador=${rut}`
                                        }).then(response => response.json());
                                    }, data.payRut);
                                    const nombrePagador = json.extNombres + ' ' + json.extApellidoPat + ' ' + json.extApellidoMat;
                                    const resp = await newPage.evaluate(async name => {
                                        $('#NombrePagador').val(name);
                                        var url = urlAjax('bono', 'pagar');
                                        return await fetch(url, {
                                            method: 'POST',
                                            headers: new Headers({ 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' }),
                                            body: $('#FrmPagar').serialize()
                                        }).then(response => response.json());
                                    }, nombrePagador);
                                    if (resp.codigo === 0) {
                                        var array = await newPage.evaluate(() => {
                                            return {
                                                total: $('#FrmPagar > div:nth-child(1) > table > tbody > tr:nth-child(8) > td').html().replace(/\D/g, ''),
                                                copago: $('#ValorCopago').val()
                                            };
                                        });
                                        array.tokenWs = resp.datos.token;
                                        await browser.close();
                                        end(JSON.stringify(array), true);
                                    } else {
                                        await browser.close();
                                        end(`{ TBK_ORDEN_COMPRA: ${resp.datos}, mensaje: ${resp.mensaje} }`, true);
                                    }
                                    break;
                                default:
                                    var err = "action:" + params.action + " not recognized";
                                    console.log(err);
                                    await browser.close();
                                    end(err);
                                    break;
                            }
                        });
                    });
                    await submitCatpcha(page, data.rut, captcha)
                        .catch(e => end(e));
                })
                .catch(e => end(e));
        }).catch(e => end(119 + e));
};