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

//data { acKey, browserWSEndpoint, rut }

module.exports = async function (callback, data) {
    if (!data.browserWSEndpoint) {
        if (!data.rut) callback("No value for RUT", null);
        console.log(`No browserWSEndpoint Starting Service with RUT:${data.rut}`);
        const browser = await puppeteer.launch({ headless: true });
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
                        await fs.rename(file_path, path.join(__dirname, '..', 'src', 'scripts', 'node', 'mi', 'trainset', `${captcha}.png`), function (err) {
                            if (err) callback(err, null);
                        });
                        callback(null, browser.wsEndpoint());
                        break;
                }
            });
        });
        await submitCatpcha(page, data.rut, captcha).catch(error => { callback(error, null); });
    } else {
        const browserWSEndpoint = data.browserWSEndpoint;
        const browser = await puppeteer.connect({ browserWSEndpoint: browserWSEndpoint });
        var page = (await browser.pages())[0];
        const user = await page.evaluate(async (rut) => {
            var url = urlAjax('bono', 'execWSCertifPagador');
            return await fetch(url, {
                method: 'POST',
                headers: new Headers({ 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' }),
                body: `RutPagador=${rut}`
            }).then(response => response.text());
        }, data.rut);
        if (user === 'ERROR') callback(user, null);
        callback(null, user);
    }
};