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
        rutRecep: "76998275",
        dvRecep: "2",
        total: "144000",
        signature: "0076"
    };
    const browser = await puppeteer.launch({
        headless: false
    });
    const page = (await browser.pages())[0];
    await page.goto('https://zeusr.sii.cl/AUT2000/InicioAutenticacion/IngresoRutClave.html?https://www1.sii.cl/cgi-bin/Portal001/mipeSelEmpresa.cgi?DESDE_DONDE_URL=OPCION%3D34%26TIPO%3D4', { waitUntil: 'networkidle2' });
    await type(page, '#rutcntr', data.rut);
    await type(page, '#clave', data.pwd);
    await page.click('button[title="Ingresar"]');
    await page.waitForSelector('select[name="RUT_EMP"]');

    await page.select('select[name="RUT_EMP"]', data.rutEmp);
    await page.click('button[type="submit"]');
    await page.waitForSelector('#EFXP_RUT_RECEP');

    await type(page, '#EFXP_RUT_RECEP', data.rutRecep);
    await type(page, '#EFXP_DV_RECEP', data.dvRecep);
    page.on('response', async response => {
        if (response.url() === 'https://www1.sii.cl/cgi-bin/Portal001/mipeGenFacEx.cgi?' && response.status() === 200) {
            await page.waitForSelector('input[name="EFXP_RZN_SOC_RECEP"]');
            await page.waitForFunction(() => document.querySelector('input[name="EFXP_RZN_SOC_RECEP"]').value.length !== 0);

            await type(page, 'input[name="EFXP_NMB_01"]', "servicios profesionales");
            await type(page, 'input[name="EFXP_QTY_01"]', "1");
            await type(page, 'input[name="EFXP_PRC_01"]', data.total);
            await page.click('button[name="Button_Update"]');
            await page.waitForSelector('input[name="btnSign"]');

            await page.click('input[name="btnSign"]');
            await page.waitForSelector('#myPass');

            await type(page, '#myPass', data.signature);
            //await page.click('#btnFirma');
        }
    });
    await page.$eval('#EFXP_DV_RECEP', e => e.blur());
};