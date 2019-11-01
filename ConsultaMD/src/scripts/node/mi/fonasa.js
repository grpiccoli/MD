'use strict';
const puppeteer = require('puppeteer');
const anticaptchaAsync = require('anticaptcha-async');
const fs = require('fs');
const path = require('path');
const querystring = require('querystring');

//const rut = '16124902-5';
//const phone = '968419339';
//const email = 'guille.arp@gmail.com';
//const docRut = '12116504-K';
//const specialty = '120';
//const region = '10';
//const commune = '10101';
//const payRut = '16124902-5';
//const acKey = '693c4e031bcd23937811cedd2f1dba08';

module.exports = async function (callback, rut, phone, email, docRut, specialty, region, commune, payRut, acKey) {
    const client = anticaptchaAsync(acKey);
    const selector = '#captcha';
    const png = 'tmp.png';
    const browser = await puppeteer.launch({ headless: false });
    const page = await browser.newPage();
    page.setViewport({ width: 1000, height: 600, deviceScaleFactor: 1 });
    await page.goto('https://bonowebfon.fonasa.cl/', { waitUntil: 'networkidle2' });
    async function screenshotDOMElement(opts) {
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
    }
    const file_path = path.join(__dirname, png);

    async function readCaptcha(file_path, png, selector) {
        await screenshotDOMElement({
            imgPath: png,
            selector: selector
        });
        const result = await client.getImage(fs.createReadStream(file_path));
        console.log('captcha:' + result.getValue());
        return result.getValue();
    }

    let captcha = await readCaptcha(file_path, png, selector);

    async function submitCatpcha(rut, captcha) {
        await page.type('#RutBeneficiario', rut);
        await page.type('#captcha_code', captcha);
        await page.click('#btnCertifBenef');
    }

    page.on('response', (response) => {
        console.log('url:'+response.url());
        let url = response.url();
        let params = querystring.decode(url.split('?')[1]);
        response.text().then(async function (textBody) {
            console.log('body:'+textBody);
            console.log('action:'+params.action);
            switch (params.action) {
                case 'execWSCertifTrab':
                    if (textBody === 'ERROR_CAPTCHA') {
                        await page.click('button[data-dismiss="modal"]');
                        await page.click('a.btn.btn-default');
                        captcha = await readCaptcha(file_path, png, selector);
                        await submitCatpcha(rut, captcha);
                    }
                    fs.rename(file_path, path.join(__dirname, 'trainset', `${captcha}.png`), function (err) {
                        if (err) console.log(`ERROR: ${err}`);
                    });
                    if (type === 'validate') callback(null, textbody);
                    await page.type('#CelularNotificacion', phone);
                    await page.type('#RepiteCelular', phone);
                    await page.type('#EmailNotificacion', email);
                    await page.select('#TipoBusquedaProfesional', 'RutProfesional');
                    await page.type('#PalabraClave', docRut);
                    await page.select('#Especialidad', specialty);
                    await page.select('#Region', region);
                    break;
                case 'BwPrmComunaS':
                    await page.select('#Comuna', commune);
                    await page.click('#btnBuscaProfesional');
                    break;
                case 'BuscaPorProfesional':
                    await page.click('.btnSeleccionaAtencion:first-of-type');
                    break;
                case 'validaCAT':
                    const newTarget = await browser.waitForTarget(target => target.url().startsWith('https://bonowebfon.fonasa.cl/index.php?controller=bono&action=pago'));
                    const newPage = await newTarget.page();
                    await newPage.type('#RutPagador', payRut);
                    await newPage.click('#btnEmitir');
                    break;
                default:
                    console.log(params.action);
                    break;
            }
        });
    });
    await submitCatpcha(rut, captcha);
};