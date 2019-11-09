'use strict';
const puppeteer = require('puppeteer');
const anticaptchaAsync = require('anticaptcha-async');
const fs = require('fs');
const path = require('path');
const querystring = require('querystring');

const rut = '11927437-0';
const pass = 'guillermo';
const drName = 'Hugo Alexis Menares Rodriguez';

const type = async function (page, selector, value) {
    await page.evaluate(selector => { document.querySelector(selector).value = ''; }, selector);
    await page.type(selector, value);
};

(async () => {
    const browser = await puppeteer.launch({
        headless: false,
        slowMo: 1000
    });
    const page = await browser.newPage();
    page.setViewport({ width: 1000, height: 600, deviceScaleFactor: 1 });
    await page.goto('https://www.isaprebanmedica.cl/LoginBanmedica.aspx', { waitUntil: 'networkidle2' });
    page.on('response', async (response) => {
        console.log('url:'+response.url());
        let url = response.url();
        let urlQuery = url.split('?');
        let urlBase = urlQuery[0];
        switch (urlBase) {
            case "https://www.isaprebanmedica.cl/":
                await page.goto('/bonoWeb/Index.aspx', { waitUntil: 'networkidle2' });
                break;
            case "https://www.isaprebanmedica.cl/bonoWeb/Index.aspx":
                await page.click('#busqueda-beneficiario-button');
                await page.waitForSelector('//li.ui-menu-item[contains(.,"Jorge Alejandro Muñoz Brand")]');
                await page.click('//li.ui-menu-item[contains(.,"Jorge Alejandro Muñoz Brand")]');
                await type(page, '#busqueda-query', drName);
                await page.goto('/bonoWeb/Index.aspx', { waitUntil: 'networkidle2' });
                break;
            case "https://www.isaprebanmedica.cl/bonosweb/home/search":
                let json = response.json();
                if (json.data.length === 0) {
                    await type(page, '#busqueda-query', drName);
                    await page.goto('/bonoWeb/Index.aspx', { waitUntil: 'networkidle2' });
                }
                break;
        }
    });
    await page.type('#txt_rut', rut);
    await page.type('#txt_pass', pass);
    await page.click('#lnkbtn_login');
})();