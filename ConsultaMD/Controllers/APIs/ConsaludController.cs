using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using ConsultaMD.Data;
using ConsultaMD.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace ConsultaMD.Controllers
{
    [Authorize]
    public class ConsaludController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly int _waitTime;
        private readonly IStringLocalizer<ConsaludController> _localizer;
        public ConsaludController(
            IHostingEnvironment hostingEnvironment,
            UserManager<ApplicationUser> userManager,
            IStringLocalizer<ConsaludController> localizer,
            ApplicationDbContext context)
        {
            _waitTime = 60;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
            _context = context;
            _localizer = localizer;
        }

        [HttpPost]
        [ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Consalud(string rut, string pwd)
        {
            rut = Regex.Replace(rut, @"\.", "");

            var getCookies = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler
            {
                CookieContainer = getCookies,
                UseCookies = true,
                UseDefaultCredentials = false
            };

            using (HttpClient getclient = new HttpClient(handler))
            {
                var login = new Uri("https://clientes.consalud.cl/Default.aspx");

                using (await getclient.GetAsync(login).ConfigureAwait(false))
                {
                    var loginCookie = getCookies.GetCookies(login).Cast<System.Net.Cookie>().FirstOrDefault();

                    var setCookies = new CookieContainer();
                    using (HttpClientHandler setHandler = new HttpClientHandler
                    {
                        CookieContainer = setCookies,
                        UseCookies = true,
                        UseDefaultCredentials = false
                    })
                    {
                        var uri = new Uri("https://clientes.consalud.cl/auw.aspx");
                        setCookies.Add(uri, loginCookie);

                        var loginFormValues = new Dictionary<string, string>
                        {
                            { "esWeb", "1" },
                            { "usr", rut },
                            { "pwd", pwd },
                            { "ingresar", "Ingresar" }
                        };
                        var formContent = new FormUrlEncodedContent(loginFormValues);

                        using (HttpClient client = new HttpClient(handler))
                        {
                            handler.Dispose();
                            client.DefaultRequestHeaders.Add("Referer", "https://clientes.consalud.cl/Default.aspx");
                            using (var response = await client.PostAsync(uri, formContent).ConfigureAwait(false))
                            {
                                formContent.Dispose();
                                var parser = new HtmlParser();
                                using (var doc = await parser.ParseDocumentAsync(await response.Content.ReadAsStringAsync().ConfigureAwait(false)).ConfigureAwait(false))
                                {
                                    var test = doc.GetElementById("esWeb");
                                    if (test == null)
                                    {
                                        return Ok();
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return NotFound();
        }
        //, string placeId
        public async Task<IActionResult> Sbono(string rutp, string rutdr)
        {
            var signin = new Uri("https://clientes.consalud.cl/");
            var ventabonos = new Uri("https://clientes.consalud.cl/VentaBonos/contenido_html/index.aspx#/atenciones-medicas");
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            var patient = _context.Patients.SingleOrDefault(p => p.NaturalId == user.PersonId);
            var home = _hostingEnvironment.ContentRootPath;
            var service = ChromeDriverService.CreateDefaultService(home);
            var Options = new ChromeOptions();
            var rutbGuion = user.UserName.Replace(".", "", StringComparison.InvariantCulture);
            var rutpNoDV = rutp?.Replace(".", "", StringComparison.InvariantCulture).Split("-")[0];
            Options.AddArguments(
                //"headless",
                //"disable-gpu", "blink-settings=imagesEnabled=false"
            );
            var driver = new ChromeDriver(service, Options);
            service.Dispose();
            driver.Navigate().GoToUrl(signin);

            Login(driver, rutbGuion, patient.InsurancePassword);

            ChoseMedical(driver, ventabonos, signin, rutbGuion, patient.InsurancePassword);

            var userSelect = new SelectElement(driver.FindElementByCssSelector("#beneficiario-ubicacion select"));
            userSelect.SelectByValue(rutpNoDV);

            driver.FindElementById("welcome-submit").Click();

            new WebDriverWait(driver, TimeSpan.FromSeconds(_waitTime))
                .Until(SeleniumExtras.WaitHelpers
                .ExpectedConditions.ElementIsVisible(By.Id("content")));

            new WebDriverWait(driver, TimeSpan.FromSeconds(_waitTime)).Until(
                d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));

            var regionSelect = new SelectElement(driver
                .FindElementByCssSelector("#content > div:nth-child(5) > div > div > div > div:nth-child(1) > select"));
            regionSelect.SelectByText(_localizer["LOS LAGOS"]); //***

            new WebDriverWait(driver, TimeSpan.FromSeconds(_waitTime)).Until(
                d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));

            var comunaSelect = new SelectElement(driver
                .FindElementByCssSelector("#content > div:nth-child(5) > div > div > div > div:nth-child(1) > select"));
            comunaSelect.SelectByText(_localizer["PUERTO MONTT"]); //***

            new WebDriverWait(driver, TimeSpan.FromSeconds(_waitTime)).Until(
                d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));

            new WebDriverWait(driver, TimeSpan.FromSeconds(_waitTime))
                .Until(SeleniumExtras.WaitHelpers
                .ExpectedConditions.ElementToBeClickable(By
                .CssSelector("#busqueda-atenciones > div.panel.form-filter.equal-height-item.col-md-7.col-md-offset-1 > h3 > a")));

            driver.FindElementByCssSelector("#busqueda-atenciones > div.panel.form-filter.equal-height-item.col-md-7.col-md-offset-1 > h3 > a").Click();

            new WebDriverWait(driver, TimeSpan.FromSeconds(_waitTime))
                .Until(SeleniumExtras.WaitHelpers
                .ExpectedConditions.ElementToBeClickable(By
                .CssSelector("#filtro-medico > div:nth-child(3) > div > a")));

            driver.FindElementByCssSelector("#filtro-medico > div:nth-child(3) > div > a").Click();

            new WebDriverWait(driver, TimeSpan.FromSeconds(_waitTime))
                .Until(SeleniumExtras.WaitHelpers
                .ExpectedConditions.ElementIsVisible(By.Id("s2id_autogen3_search")));

            driver.FindElementById("s2id_autogen3_search").SendKeys(rutdr);

            new WebDriverWait(driver, TimeSpan.FromSeconds(_waitTime)).Until(
                d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));

            new WebDriverWait(driver, TimeSpan.FromSeconds(_waitTime))
                .Until(SeleniumExtras.WaitHelpers
                .ExpectedConditions.ElementToBeClickable(By
                .Id("select2-results-3")));

            driver.FindElementById("select2-results-3").Click();

            new WebDriverWait(driver, TimeSpan.FromSeconds(_waitTime))
                .Until(SeleniumExtras.WaitHelpers
                .ExpectedConditions.ElementToBeClickable(By
                .CssSelector("#filtro-medico > button")));

            driver.FindElementByCssSelector("#filtro-medico > button").Click();

            new WebDriverWait(driver, TimeSpan.FromSeconds(_waitTime))
                .Until(SeleniumExtras.WaitHelpers
                .ExpectedConditions.ElementIsVisible(By
                .CssSelector("#content > table")));

            driver.Dispose();

            return Ok();
        }
        public static void Login(ChromeDriver driver, string rut, string pwd)
        {
            if(driver != null)
            {
                new WebDriverWait(driver, TimeSpan.FromSeconds(30))
                    .Until(SeleniumExtras.WaitHelpers
                    .ExpectedConditions.ElementIsVisible(By.Id("usr")));

                driver.FindElementById("usr").SendKeys(rut);
                driver.FindElementById("pwd").SendKeys(pwd);

                new WebDriverWait(driver, TimeSpan.FromSeconds(30))
                    .Until(SeleniumExtras.WaitHelpers
                    .ExpectedConditions.ElementToBeClickable(By.Id("ingresar")));
                driver.FindElementById("ingresar").Click();

                new WebDriverWait(driver, TimeSpan.FromSeconds(30))
                    .Until(SeleniumExtras.WaitHelpers
                    .ExpectedConditions.ElementIsVisible(By.Id("[%id_pagina%]")));
            }
        }
        public static void ChoseMedical(ChromeDriver driver, Uri ventabonos, Uri signin, string rut, string pwd)
        {
            while(driver?.Url != ventabonos?.ToString())
            {
                driver.Navigate().GoToUrl(ventabonos);

                try
                {
                    driver.FindElementByCssSelector("#loading > a");
                    new WebDriverWait(driver, TimeSpan.FromSeconds(30))
                        .Until(SeleniumExtras.WaitHelpers
                        .ExpectedConditions.ElementToBeClickable(By.CssSelector("#loading > a")));
                    driver.FindElementByCssSelector("#loading > a").Click();
                    if (driver.Url == signin?.ToString())
                    {
                        Login(driver, rut, pwd);
                    }
                    continue;
                }
                catch
                {
                    new WebDriverWait(driver, TimeSpan.FromSeconds(30))
                        .Until(SeleniumExtras.WaitHelpers
                        .ExpectedConditions.ElementToBeClickable(By.Id("div-atenciones-medicas")));

                    driver.FindElementById("div-atenciones-medicas").Click();

                    new WebDriverWait(driver, TimeSpan.FromSeconds(30))
                        .Until(SeleniumExtras.WaitHelpers
                        .ExpectedConditions.ElementToBeClickable(By.Id("welcome-submit")));
                    throw;
                }
            }
        }
    }
}