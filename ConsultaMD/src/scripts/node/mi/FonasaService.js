'use strict';
const puppeteer = require('puppeteer');

const initBrowser = async () => {
    const browser = await puppeteer.launch(
        { ignoreHTTPSErrors: true, headless: true, args: ['--no-sandbox', '--disable-setuid-sandbox'] }
    );
    const page = (await browser.pages())[0];
    await page.goto('https://bonowebfon.fonasa.cl/', { waitUntil: 'networkidle2' });
    return [null, browser];
};

const readInfo = async (browser, person, docData) => {
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

module.exports = async (callback, data) => {
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
                let values = await initBrowser();
                if (values[0]) callback(values[0], null);
                let user = await readInfo(values[1], data.rut, data.docData);
                if (user.indexOf('ERROR') !== -1) callback(user + error, null);
                callback(null, user);
            });
    }
};