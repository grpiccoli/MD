'use strict';
const puppeteer = require('puppeteer');
const anticaptchaAsync = require('anticaptcha-async');
const fs = require('fs');
const path = require('path');
const querystring = require('querystring');

const png = 'tmp.png';
const file_path = path.join(__dirname, png);
const selector = '#captcha';

const screenshotDOMElement = async function (page, opts) {
    const padding = 'padding' in opts ? opts.padding : 0;
    const imgPath = 'imgPath' in opts ? opts.imgPath : null;
    const selector = opts.selector;

    if (!selector)
        throw Error('Please provide a selector.');

    const rect = await page.evaluate((selector) => {
        const element = document.querySelector(selector);
        if (!element)
            return null;
        const drect = element.getBoundingClientRect();
        return { left: drect.x, top: drect.y, width: drect.width, height: drect.height };
    }, selector);

    if (!rect)
        throw Error(`Could not find element that matches selector: ${selector}.`);

    return await page.screenshot({
        path: imgPath,
        clip: {
            x: rect.left - padding,
            y: rect.top - padding,
            width: rect.width + padding * 2,
            height: rect.height + padding * 2
        }
    });
};

const readCaptcha = async function (page, acKey) {
    await screenshotDOMElement(page, {
        imgPath: file_path,
        selector: selector
    }).catch(error => { callback(error, null); });
    const client = anticaptchaAsync(acKey);
    const result = await client.getImage(fs.createReadStream(file_path));
    return result.getValue();
};

const type = async function (page, selector, value) {
    await page.evaluate(selector => { document.querySelector(selector).value = ''; }, selector);
    await page.type(selector, value);
};

const submitCatpcha = async function (page, rut, captcha) {
    await type(page, '#RutBeneficiario', rut);
    await type(page, '#captcha_code', captcha);
    await page.click('#btnCertifBenef');
};

//data:
//rut, phone, email, docRut, specialty, region, commune, payRut, acKey

const isWin = process.platform === "win32";
const preUnix = '/root/webapps/consultamd/';
const preWin = '../../../../';
const chrome_path = 'node_modules/puppeteer/.local-chromium/';
const ver = '706915';
const win = preWin + 'win64-' + ver + '/chrome-win/chrome.exe';
const unix = preUnix + 'linux-' + ver + '/chrome-linux/chrome';

module.exports = async function (callback, data) {
    const browser = await puppeteer.launch(
        {
            ignoreHTTPSErrors: true,
            headless: true,
            args: ['--no-sandbox', '--disable-setuid-sandbox']
            //,
            //executablePath: isWin ? win : unix
        }
    );
    const page = (await browser.pages())[0];
    page.setViewport({ width: 1000, height: 600, deviceScaleFactor: 1 });
    await page.goto('https://bonowebfon.fonasa.cl/', { waitUntil: 'networkidle2' });
    let captcha = await readCaptcha(page, data.acKey).catch(error => { callback(error, null); });
    page.on('response', (response) => {
        let url = response.url();
        let params = querystring.decode(url.split('?')[1]);
        response.text().then(async function (textBody) {
            switch (params.action) {
                case 'execWSCertifTrab':
                    if (textBody === 'ERROR_CAPTCHA') {
                        await page.reload({ waitUntil: ['networkidle0', 'domcontentloaded'] });
                        captcha = await readCaptcha(page, data.acKey).catch(error => { callback(error, null); });
                        await submitCatpcha(page, data.rut, captcha).catch(error => { callback(error, null); });
                        break;
                    }
                    await fs.rename(file_path, path.join(__dirname, 'trainset', `${captcha}.png`), function (err) {
                        if (err) callback(err, null);
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
                    const newTarget = await browser.waitForTarget(target => target.url().startsWith('https://bonowebfon.fonasa.cl/index.php?controller=bono&action=pago'));
                    const newPage = await newTarget.page();
                    await newPage.type('#RutPagador', data.payRut);
                    const json = await newPage.evaluate(async (rut) => {
                        var url = urlAjax('bono', 'execWSCertifPagador');
                        return await fetch(url, {
                            method: 'POST',
                            headers: new Headers({ 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' }),
                            body: `RutPagador=${rut}`
                        }).then(response => response.json());
                    }, data.payRut);
                    const nombrePagador = json.extNombres + ' ' + json.extApellidoPat + ' ' + json.extApellidoMat;
                    const resp = await newPage.evaluate(async (name) => {
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
                        callback(null, JSON.stringify(array));
                    } else {
                        await browser.close();
                        callback(null, `{ TBK_ORDEN_COMPRA: ${resp.datos}, mensaje: ${resp.mensaje} }`);
                    }
                    break;
                default:
                    var err = "action:" + params.action + " not recognized";
                    console.log(err);
                    await browser.close();
                    callback(err, null);
                    break;
            }
        });
    });
    await submitCatpcha(page, data.rut, captcha).catch(error => { callback(error, null); });
};