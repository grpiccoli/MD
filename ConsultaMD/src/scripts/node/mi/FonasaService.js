'use strict';
const puppeteer = require('puppeteer');
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
        await page.goto('https://bonowebfon.fonasa.cl/', { waitUntil: 'networkidle2' }).catch(e => { throw e; });
    }).catch(e => { throw e; });
};

const readInfo = async data => {
    var page = (await brw.pages())[0];
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
        }).catch(e => { throw e; });
    }, data.rut, data.docData).catch(e => { throw e; });
    var result = JSON.stringify(user);
    if (result.indexOf('ERROR') !== -1) end(result);
    return result;
};

//data { acKey, docData, rut }
module.exports = async (callback, data) => {
    cllbck = callback;
    await initBrowser().then(async () => 
        await readInfo(data)
            .then(user => end(user, true))
            .catch(e => end(66 + e))
    ).catch(e => end(66 + e));
};