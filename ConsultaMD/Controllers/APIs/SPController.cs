﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium.Chrome;
using Tesseract;

namespace ConsultaMD.Controllers
{
    [Authorize]
    [Produces("application/json")]
    //Servicios Públicos
    public class SPController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public SPController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        [HttpPost]
        [ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //val  : name| rut | sex | address | commune | all
        //type : rut | buscar
        public async Task<IActionResult> GetNRYF(int rut, string dv, string val, string type)
        {
            var rutF = $"{rut.ToString("N0", new CultureInfo("es-CL"))}-{dv}";
            var loginFormValues = new Dictionary<string, string>
            {
                { "term", rutF }
            };
            var formContent = new FormUrlEncodedContent(loginFormValues);

            CookieContainer getCookies = new CookieContainer();
            HttpClientHandler getHandler = new HttpClientHandler
            {
                CookieContainer = getCookies,
                UseCookies = true,
                UseDefaultCredentials = false
            };
            var uri = new Uri($"https://www.nombrerutyfirma.com/{type}");

            using (HttpClient client = new HttpClient(getHandler))
            {
                using (var response = await client.PostAsync(uri, formContent))
                {
                    var parser = new HtmlParser();
                    using (var doc = await parser.ParseDocumentAsync(await response.Content.ReadAsStringAsync()))
                    {
                        var result = doc.QuerySelectorAll("tbody > tr > td");
                        if (result.Any())
                        {
                            var num = 0;
                            switch (val)
                            {
                                case "name":
                                    var names = result[num].InnerHtml.ToUpper().Split(" ");
                                    var name = $"{string.Join(" ", names.Skip(2))} {string.Join(" ", names.Take(2))}";
                                    return Ok(name);
                                case "rut":
                                    num = 1;
                                    break;
                                case "sex":
                                    num = 2;
                                    break;
                                case "address":
                                    num = 3;
                                    break;
                                case "comune":
                                    num = 4;
                                    break;
                                case "all":
                                    return Ok(new
                                    {
                                        name = result[0].InnerHtml.ToUpper(),
                                        rut = result[1].InnerHtml,
                                        sex = result[2].InnerHtml,
                                        address = result[3].InnerHtml,
                                        comune = result[4].InnerHtml
                                    });
                            }
                            return Ok(result[num].InnerHtml);
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                }
            }
        }
        [HttpPost]
        [ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //val : rut | razon_social | actividades | all
        public async Task<IActionResult> GetSII(int rut, string dv, string val)
        {
           using (HttpClient client = new HttpClient())
            {
                using (var response = await client.GetAsync($"https://siichile.herokuapp.com/consulta?rut={rut}{dv}"))
                {
                    var json = JObject.Parse(await response.Content.ReadAsStringAsync());
                    if (string.IsNullOrEmpty((string)json.SelectToken("razon_social"))) return NotFound();
                    return val == "all" ? Ok(json) : Ok(json.SelectToken(val));
                }
            }
        }
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DocumentStatus(string rut)
        {
            var uri = new Uri("https://portal.sidiv.registrocivil.cl/usuarios-portal/pages/DocumentStatus.xhtml");
            using (HttpClient hc = new HttpClient())
            {
                var parser = new HtmlParser();
                using (HttpClient client = new HttpClient())
                {
                    using (var docHome = await parser.ParseDocumentAsync(await client.GetStringAsync(uri)))
                    {
                        var loginFormValues = new Dictionary<string, string>
                        {
                            { "form", "form" },
                            { "form:run", rut },
                            { "form:styledSelect", "CEDULA" },
                            { "form:buttonHidden", "" },
                            { "javax.faces.ViewState", Javax(docHome) }
                        };
                        var formContent = new FormUrlEncodedContent(loginFormValues);
                        using (var qNac = await client.PostAsync(uri, formContent))
                        {
                            using (var docNac = await parser.ParseDocumentAsync(await qNac.Content.ReadAsStringAsync()))
                            {
                                var tableNac = GetRows(docNac);
                                if (tableNac.Length > 2) return Ok(new { nacionalidad= "chilena"});
                                loginFormValues["javax.faces.ViewState"] = Javax(docNac);
                                loginFormValues["form:styledSelect"] = "CEDULA_EXT";
                                formContent = new FormUrlEncodedContent(loginFormValues);
                                using (var qExt = await client.PostAsync(uri, formContent))
                                {
                                    using (var docExt = await parser.ParseDocumentAsync(await qExt.Content.ReadAsStringAsync()))
                                    {
                                        var tableExt = GetRows(docExt);
                                        if (tableExt.Length > 2) return Ok(new { nacionalidad= "extranjera" });
                                        return NotFound();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DocumentRequestStatus(int rut, string dv, int carnet)
        {
            if (rut > 900_000 && rut < 30_000_000 && dv.Length == 1 && carnet > 100_000_000 && carnet < 999_999_999)
            {
                var uri = new Uri("https://portal.sidiv.registrocivil.cl/usuarios-portal/pages/DocumentRequestStatus.xhtml");
                using (HttpClient hc = new HttpClient())
                {
                    var parser = new HtmlParser();
                    using (HttpClient client = new HttpClient())
                    {
                        using (var docHome = await parser.ParseDocumentAsync(await client.GetStringAsync(uri)))
                        {
                            var loginFormValues = new Dictionary<string, string>
                        {
                            { "form", "form" },
                            { "form:run", $"{rut}-{dv}" },
                            { "form:selectDocType", "CEDULA" },
                            { "form:docNumber", carnet.ToString() },
                            { "form:buttonHidden", "" },
                            { "javax.faces.ViewState", Javax(docHome) }
                        };
                            var formContent = new FormUrlEncodedContent(loginFormValues);
                            using (var qNac = await client.PostAsync(uri, formContent))
                            {
                                using (var docNac = await parser.ParseDocumentAsync(await qNac.Content.ReadAsStringAsync()))
                                {
                                    var estado = GetEstadoCarnet(docNac);
                                    if (estado == "Vigente") return Ok(new { nacionalidad = "CHILENA" });
                                    loginFormValues["javax.faces.ViewState"] = Javax(docNac);
                                    loginFormValues["form:selectDocType"] = "CEDULA_EXT";
                                    formContent = new FormUrlEncodedContent(loginFormValues);
                                    using (var qExt = await client.PostAsync(uri, formContent))
                                    {
                                        using (var docExt = await parser.ParseDocumentAsync(await qExt.Content.ReadAsStringAsync()))
                                        {
                                            estado = GetEstadoCarnet(docExt);
                                            if (estado == "Vigente") return Ok(new { nacionalidad = "EXTRANJERA" });
                                            return NotFound();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return NotFound();
        }
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Donante(string rut)
        {
            var uri = new Uri("https://consultarnnd.srcei.cl/ConsultaRNND/");

            var service = ChromeDriverService.CreateDefaultService(_hostingEnvironment.ContentRootPath);
            var Options = new ChromeOptions();
            //Options.AddArguments("headless", "disable-gpu");
            using (var driver = new ChromeDriver(service, Options)) {
                driver.Navigate().GoToUrl(uri);
                var urlCaptcha = driver.FindElementById("simplecaptchaimg").GetAttribute("src");
                await DownloadAsync(new Uri(urlCaptcha), "solveCaptcha.jpg");
                var text = GetText("solveCaptcha.jpg");
                var result = driver
                    .ExecuteAsyncScript($"dwr.engine._execute(ConsultaWeb._path, 'ConsultaWeb', 'verificaDonante', '{rut}', '{text}', function(d){{return d;}})");
                driver.Quit();
                switch (result)
                {
                    case "Donante":
                    case "No donante":
                    case "Menor de edad":
                        return Ok(new { status = result });
                    default:
                        return NotFound();
                }
            }
        }
        public static async Task DownloadAsync(Uri requestUri, string filename)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Get, requestUri))
            using (var send = await client.SendAsync(request))
            using (
                Stream contentStream = await (send).Content.ReadAsStreamAsync(),
                stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None, 3145728, true))
            {
                await contentStream.CopyToAsync(stream);
            }
        }
        public static string GetText(string imgsource)
        {
            var ocrtext = string.Empty;
            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
            {
                using (var img = Pix.LoadFromFile(imgsource))
                {
                    using (var page = engine.Process(img))
                    {
                        ocrtext = page.GetText();
                    }
                }
            }
            return ocrtext;
        }
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ValidateRUT(int rut, string dv)
        {
            if(rut > 900_000 && rut < 30_000_000 && dv.Length == 1)
            {
                var sii = await GetSII(rut, dv, "razon_social");
                if (sii.GetType() == typeof(NotFoundResult))
                {
                    var servel = await GetNRYF(rut, dv, "name", "rut");
                    if (servel.GetType() == typeof(NotFoundResult))
                    {
                        return NotFound();
                    }
                    return Ok(servel);
                }
                else
                {
                    return Ok(sii);
                }
            }
            return NotFound();
        }

        public IHtmlCollection<IElement> GetRows(IHtmlDocument doc)
        {
            return doc.QuerySelectorAll("tr.rowWidth > td");
        }

        public string GetEstadoCarnet(IHtmlDocument doc)
        {
            return doc.QuerySelector(".setWidthOfSecondColumn").TextContent;
        }

        public string Javax(IHtmlDocument doc)
        {
            return doc.GetElementsByTagName("input").Last().GetAttribute("value");
        }

        public async Task<IHtmlDocument> GetDoc(string rep)
        {
            using (HttpClient hc = new HttpClient())
            {
                var parser = new HtmlParser();
                return await parser.ParseDocumentAsync(await hc.GetStringAsync(rep));
            }
        }

        public async Task<IHtmlDocument> GetDocStream(string rep)
        {
            using (HttpClient hc = new HttpClient())
            {
                var parser = new HtmlParser();
                return await parser.ParseDocumentAsync(await hc.GetStreamAsync(rep));
            }
        }

        public async Task<HtmlDocument> GetDocXPath(string rep)
        {
            using (HttpClient hc = new HttpClient())
            {
                var doc = new HtmlDocument();
                doc.Load(await hc.GetStreamAsync(rep));
                return doc;
            }
        }

        public IActionResult ParseName(string full, bool lastfirst)
        {
            var names = full.Split(" ");
            return lastfirst ?
                Ok(new
                {
                    names = string.Join(" ", names.Skip(2)),
                    psur = names[0],
                    msur = names[1]
                }) :
                Ok(new
                {
                    names = string.Join(" ", names.Take(names.Count() - 2).ToArray()),
                    psur = names[names.Count() - 1],
                    msur = names[names.Count() - 2]
                });
        }
    }
}