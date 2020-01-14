'use strict';
const puppeteer = require('puppeteer');
const anticaptchaAsync = require('anticaptcha-async');
const fs = require('fs');
const path = require('path');

const png = 'tmp.png';
const file_path = path.join(__dirname, png);
const selector = '#form\\:captchaPanel > img';

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

const initBrowser = async (acKey) => {
    const browser = await puppeteer.launch(
        { headless: true }
    );
    const page = (await browser.pages())[0];
    await page.goto('https://portal.sidiv.registrocivil.cl/usuarios-portal/pages/DocumentRequestStatus.xhtml', { waitUntil: 'networkidle2' });
    //CAPTCHA SOLVING
    let captcha = await readCaptcha(page, acKey).catch(error => { return [error, null]; });
    return [null, browser, captcha];
};

const readInfo = async (browser, data) => {
    var page = (await browser.pages())[0];
    var suffix = data.isExt ? '_EXT' : '';
    var type = `CEDULA${suffix}`;
    var close = false;
    var isVigente = await page.evaluate(async (captcha, rut, carnet, type) => {
        var url = 'https://portal.sidiv.registrocivil.cl/usuarios-portal/pages/DocumentRequestStatus.xhtml';
        var javax = $("#javax\\.faces\\.ViewState").val();
        var body = 'form=form&form%3AcaptchaUrl=initial'
            + `&form%3Arun=${rut}`
            + `&form%3AselectDocType=${type}`
            + `&form%3AdocNumber=${carnet}`
            + `&form%3AinputCaptcha=${captcha}`
            + '&form%3AbuttonHidden='
            + `&javax.faces.ViewState=${javax}`;
        return await fetch(url, {
            method: 'POST',
            headers: new Headers({ 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' }),
            body: body
        }).then(response => {
            if (response.status === 200) {
                return response.text();
            }
            var sessionError = text.includes('Sesión no válida');
            close = sessionError;
            return vigente;
            //throw Error(`Status ${response.status} ${response.statusText}.`);
        }).then(text => {
            var vigente = text.includes('Vigente');
            if (!vigente) {
                var captchaError = text.includes('Por favor, Intente nuevamente.');
                var sessionError = text.includes('Sesión no válida');
                var errorValidation = text.includes('La información ingresada no corresponde en nuestros registros');
                close = captchaError || sessionError;
                close = !errorValidation;
            }
            return vigente;
        }).catch(err => {
            close = true;
            throw err;
        });
    }, data.captcha, data.rut, data.carnet, type);
    var user = {
        'browserWSEndpoint': browser.wsEndpoint(),
        'isValid': isVigente,
        'close': close,
        'captcha': data.captcha
    };
    return JSON.stringify(user);
};

//data:
//acKey, rut, carnet?, close, browserWSEndpoint

module.exports = async (callback, data) => {
    console.log(data);
    if (data.isExt) {
        console.log("bla");
    }
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
                let user = await readInfo(browser, data).catch(e => { throw e; });
                if (user.indexOf('ERROR') !== -1) callback(user, null);
                callback(null, user);
            })
            .catch(async error => {
                console.log(error);
                let values = await initBrowser(data.acKey).catch(e => { throw e; });
                if (values[0]) callback(values[0], null);
                data.captcha = values[2];
                let user = await readInfo(values[1], data).catch(e => { throw e; });
                if (user.indexOf('ERROR') !== -1) callback(user + error, null);
                callback(null, user);
            });
    }
};