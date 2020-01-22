'use strict';
const puppeteer = require('puppeteer');
const anticaptchaAsync = require('anticaptcha-async');
const fs = require('fs');
const path = require('path');
const querystring = require('querystring');

const png = 'tmp1.png';
const file_path = path.join(__dirname, png);
const selector = '#captcha';
const loaderRut = '0016124902-5';

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

const submitCatpcha = async function (page, captcha) {
    await type(page, '#RutBeneficiario', loaderRut);
    await type(page, '#captcha_code', captcha);
    await page.click('#btnCertifBenef');
};

const initBrowser = async function (data) {
    const browser = await puppeteer.launch({ headless: false });
    const page = (await browser.pages())[0];
    page.setViewport({ width: 1000, height: 600, deviceScaleFactor: 1 });
    await page.goto('https://bonowebfon.fonasa.cl/', { waitUntil: 'networkidle2' });
    let captcha = await readCaptcha(page, data.acKey).catch(error => { return [error, null]; });
    page.on('response', (response) => {
        let url = response.url();
        let params = querystring.decode(url.split('?')[1]);
        response.text().then(async function (textBody) {
            switch (params.action) {
                case 'execWSCertifTrab':
                    if (textBody === 'ERROR_CAPTCHA') {
                        await page.reload({ waitUntil: ['networkidle0', 'domcontentloaded'] });
                        captcha = await readCaptcha(page, data.acKey).catch(error => { return [error, null]; });
                        await submitCatpcha(page, captcha).catch(error => { return [error, null]; });
                        break;
                    }
                    await fs.rename(file_path, path.join(__dirname, 'trainset', `${captcha}.png`), function (err) {
                        if (err) return [err, null];
                    });
                    return [null, browser];
            }
        }).catch();
    });
    await submitCatpcha(page, captcha).catch(error => { return [error, null]; });
    return [null, browser];
};

const readInfo = async function (browser, person, docData) {
    var page = (await browser.pages())[0];
    var user = await page.evaluate(async (rut, doc) => {
        var url = urlAjax('bono', doc ? 'BuscaPorProfesional' : 'execWSCertifPagador');
        var body = doc ?
            `TipoBusquedaProfesional=RutProfesional&PalabraClave=${rut}&Especialidad=0&Region=0&Comuna=0`
            : `RutPagador=${rut}`;
        return await fetch(url, {
            method: 'POST',
            headers: new Headers({ 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' }),
            body: body
        }).then(response => response.text())
        .then(text => {
            try {
                return JSON.parse(text);
            } catch (err) {
                return { text: text };
            }
        });
    }, person, docData);
    user['browserWSEndpoint'] = browser.wsEndpoint();
    return JSON.stringify(user);
};

//data { acKey, browserWSEndpoint, docData, rut, close }

module.exports = async function (callback, data) {
    if (data.close) {
        await puppeteer
            .connect({ browserWSEndpoint: data.browserWSEndpoint })
            .then(async browser => {
                await browser.close();
            })
            .catch(error => {
                callback(error, null);
            });
    } else {
        await puppeteer
            .connect({ browserWSEndpoint: data.browserWSEndpoint })
            .then(async browser => {
                let user = await readInfo(browser, data.rut, data.docData);
                if (user.indexOf('ERROR') !== -1) callback(user, null);
                callback(null, user);
            })
            .catch(async error => {
                console.log(error);
                let values = await initBrowser(data);
                if (values[0]) callback(values[0], null);
                let user = await readInfo(values[1], data.rut, data.docData);
                if (user.indexOf('ERROR') !== -1) callback(user + error, null);
                callback(null, user);
            });
    }
};