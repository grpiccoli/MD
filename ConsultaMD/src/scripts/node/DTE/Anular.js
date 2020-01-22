'use strict';
const puppeteer = require('puppeteer');

const type = async function (page, selector, value) {
    await page.evaluate(selector => { document.querySelector(selector).value = ''; }, selector);
    await page.type(selector, value);
};

//data { acKey, browserWSEndpoint, docData, rut, close }

module.exports = async (callback, _data) => {
    var data = {
        rut: "161249025",
        pwd: "CF7Ja8EBYq",
        rutEmp: "76473272-3",
        folio: "64",
        signature: "0076"
    };
    const browser = await puppeteer.launch({
        headless: false
    });
    const page = (await browser.pages())[0];
    await page.goto('https://zeusr.sii.cl/AUT2000/InicioAutenticacion/IngresoRutClave.html?https://www1.sii.cl/cgi-bin/Portal001/mipeSelEmpresa.cgi?DESDE_DONDE_URL=OPCION%3D2', { waitUntil: 'networkidle2' });
    await type(page, '#rutcntr', data.rut);
    await type(page, '#clave', data.pwd);
    await page.click('button[title="Ingresar"]');
    await page.waitForSelector('select[name="RUT_EMP"]');

    await page.select('select[name="RUT_EMP"]', data.rutEmp);
    await page.click('button[type="submit"]');
    await page.waitForSelector('#tablaDatos > tbody > tr > td.sorting_1 > a');

    await page.goto(`https://www1.sii.cl/cgi-bin/Portal001/mipeAdminDocsEmi.cgi?ORDEN=&NUM_PAG=1&RUT_RECP=&FOLIO=${data.folio}&RZN_SOC=&FEC_DESDE=&FEC_HASTA=&TPO_DOC=&ESTADO=&BTN_SUBMIT=Buscar+Documentos`, { waitUntil: 'networkidle2' });
    await page.waitForSelector('#tablaDatos > tbody > tr > td.sorting_1 > a');

    await page.click('#tablaDatos > tbody > tr > td.sorting_1 > a');
    await page.waitForSelector('#my-wrapper > div.web-sii.cuerpo > div > form > div > ul:nth-child(3) > li > p > a');

    await page.click('#my-wrapper > div.web-sii.cuerpo > div > form > div > ul:nth-child(3) > li > p > a');
    await page.waitForSelector('input[name="btnSign"]');

    await page.click('input[name="btnSign"]');
    await page.waitForSelector('#myPass');

    await type(page, '#myPass', data.signature);
    //await page.click('#btnFirma');
};