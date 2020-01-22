'use strict';
const puppeteer = require('puppeteer');
const anticaptchaAsync = require('anticaptcha-async');
const fs = require('fs');
const path = require('path');

const png = 'tmp.png';
const file_path = path.join(__dirname, png);
const selector = '#form\\:captchaPanel > img';
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

    const rect = await page.evaluate((selector) => {
        const element = document.querySelector(selector);
        if (!element)
            return null;
        const drect = element.getBoundingClientRect();
        return { left: drect.x, top: drect.y, width: drect.width, height: drect.height };
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
    }).catch(e => end(38 + e) );
};

const readCaptcha = async data => {
    const page = (await brw.pages())[0];
    await screenshotDOMElement(page, {
        imgPath: file_path,
        selector: selector
    }).catch(e => end(45 + e));
    const client = anticaptchaAsync(data.acKey);
    const result = await client.getImage(fs.createReadStream(file_path))
        .catch(e => end(48 + e));
    return result.getValue();
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
        await page.goto('https://portal.sidiv.registrocivil.cl/usuarios-portal/pages/DocumentRequestStatus.xhtml', { waitUntil: 'networkidle2' });
    }).catch(e => end(e));
};

const readInfo = async data => {
    var page = (await brw.pages())[0];
    var suffix = data.isExt ? '_EXT' : '';
    var type = `CEDULA${suffix}`;
    var result = await page.evaluate(async (captcha, rut, carnet, type) => {
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
            if (text.includes('Sesión no válida'))
                throw Error(`Status ${response.status} ${response.statusText}.`);
        }).then(text => {
            var vigente = text.includes('Vigente');
            if (!vigente) {
                if (text.includes('Por favor, Intente nuevamente.'))
                    throw Error('Por favor, Intente nuevamente.');
                if (text.includes('Sesión no válida'))
                    throw Error('Sesión no válida');
                if (text.includes('La información ingresada no corresponde en nuestros registros'))
                    throw Error('La información ingresada no corresponde en nuestros registros');
            }
            return vigente.toString();
        }).catch(e => end(107 + e));
    }, data.captcha, data.rut, data.carnet, type);
    if (result.indexOf('ERROR') !== -1) end(result);
    return result;
};

//data:
//acKey, rut, carnet?
module.exports = async (callback, data) => {
    cllbck = callback;
    console.log(data);
    await initBrowser()
        .then(async () => 
            //CAPTCHA SOLVING
            await readCaptcha(data)
                .then(async c => {
                    data.captcha = c;
                    var dest = path.resolve(
                        __dirname, 'trainset', `${data.captcha}.png`);
                    await fs.rename(file_path, dest, function (err) {
                        if (err) end(err);
                    });

                    let isVigente = await readInfo(data)
                        .catch(e => end(121 + e));
                    var user = {
                        'isValid': isVigente
                    };
                    end(JSON.stringify(user), true);
                })
        ).catch(e => end(119 + e));
};
