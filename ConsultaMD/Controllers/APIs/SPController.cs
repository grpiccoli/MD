using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using ConsultaMD.Extensions;
using ConsultaMD.Models.VM;
using ConsultaMD.Services;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.NodeServices;
using Newtonsoft.Json;
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
        private readonly INodeServices _nodeServices;
        private readonly IFonasa _fonasa;
        public SPController(IHostingEnvironment hostingEnvironment,
            IFonasa fonasa,
            INodeServices nodeServices)
        {
            _fonasa = fonasa;
            _nodeServices = nodeServices;
            _hostingEnvironment = hostingEnvironment;
        }
        [HttpPost]
        [ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDoc(int rut, string dv)
        {
            if (dv == RUT.DV(rut)) {
                var search = new Uri($"http://webhosting.superdesalud.gob.cl/bases/prestadoresindividuales.nsf/(searchAll2)/Search?SearchView&Query=(FIELD%20rut_pres={rut})&Start=1&count=10");
                var id = "";
                var details = new Uri($"http://webhosting.superdesalud.gob.cl/bases/prestadoresindividuales.nsf/(searchAll2)/{id}?OpenDocument");
                return Ok();
            }
            return NotFound();
        }
        [HttpPost]
        [ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //val  : name| rut | sex | address | commune | all
        //type : rut | buscar
        public async Task<IActionResult> GetNRYF(int rut, string dv, string val, string type)
        {
            var rutF = RUT.Format(rut);
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
                getHandler.Dispose();
                using (var response = await client.PostAsync(uri, formContent).ConfigureAwait(false))
                {
                    formContent.Dispose();
                    var parser = new HtmlParser();
                    using (var doc = await parser.ParseDocumentAsync(await response.Content.ReadAsStringAsync().ConfigureAwait(false)).ConfigureAwait(false))
                    {
                        var result = doc.QuerySelectorAll("tbody > tr > td");
                        if (result.Any())
                        {
                            var num = 0;
                            switch (val)
                            {
                                case "name":
                                    var names = result[num].InnerHtml.ToUpper(new CultureInfo("es-CL")).Split(" ");
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
                                        name = result[0].InnerHtml.ToUpper(new CultureInfo("es-CL")),
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
        public async Task<IActionResult> GetSII(int run, string dv, string val)
        {
            var realDV = RUT.DV(run);
            if (realDV == dv)
            {
                var rut = RUT.Format(run);
                var response = await _nodeServices.InvokeAsync<string>("src/scripts/node/ps/SII.js", rut).ConfigureAwait(false);
                var siiData = JsonConvert.DeserializeObject<SII>(response);
                if (string.IsNullOrEmpty(siiData.Razon_Social)) return NotFound();
                return val == "all" ? Ok(response) : Ok(siiData[val]);
            }
            return NotFound();
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
                    using (var docHome = await parser.ParseDocumentAsync(await client.GetStringAsync(uri).ConfigureAwait(false)).ConfigureAwait(false))
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
                        using (var qNac = await client.PostAsync(uri, formContent).ConfigureAwait(false))
                        {
                            formContent.Dispose();
                            using (var docNac = await parser.ParseDocumentAsync(await qNac.Content.ReadAsStringAsync().ConfigureAwait(false)).ConfigureAwait(false))
                            {
                                var tableNac = GetRows(docNac);
                                if (tableNac.Length > 2) return Ok(new { nacionalidad= "chilena"});
                                loginFormValues["javax.faces.ViewState"] = Javax(docNac);
                                loginFormValues["form:styledSelect"] = "CEDULA_EXT";
                                formContent = new FormUrlEncodedContent(loginFormValues);
                                using (var qExt = await client.PostAsync(uri, formContent).ConfigureAwait(false))
                                {
                                    formContent.Dispose();
                                    using (var docExt = await parser.ParseDocumentAsync(await qExt.Content.ReadAsStringAsync().ConfigureAwait(false)).ConfigureAwait(false))
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
            if (rut > 900_000 && rut < 30_000_000 && dv?.Length == 1 && carnet > 100_000_000 && carnet < 999_999_999)
            {
                var uri = new Uri("https://portal.sidiv.registrocivil.cl/usuarios-portal/pages/DocumentRequestStatus.xhtml");
                using (HttpClient hc = new HttpClient())
                {
                    var parser = new HtmlParser();
                    using (HttpClient client = new HttpClient())
                    {
                        using (var docHome = await parser.ParseDocumentAsync(await client.GetStringAsync(uri).ConfigureAwait(false)).ConfigureAwait(false))
                        {
                            var loginFormValues = new Dictionary<string, string>
                        {
                            { "form", "form" },
                            { "form:run", $"{rut}-{dv}" },
                            { "form:selectDocType", "CEDULA" },
                            { "form:docNumber", carnet.ToString(CultureInfo.InvariantCulture) },
                            { "form:buttonHidden", "" },
                            { "javax.faces.ViewState", Javax(docHome) }
                        };
                            var formContent = new FormUrlEncodedContent(loginFormValues);
                            using (var qNac = await client.PostAsync(uri, formContent).ConfigureAwait(false))
                            {
                                formContent.Dispose();
                                using (var docNac = await parser.ParseDocumentAsync(await qNac.Content.ReadAsStringAsync().ConfigureAwait(false)).ConfigureAwait(false))
                                {
                                    var estado = GetEstadoCarnet(docNac);
                                    if (estado == "Vigente") return Ok(new { nacionalidad = "CHILENA" });
                                    loginFormValues["javax.faces.ViewState"] = Javax(docNac);
                                    loginFormValues["form:selectDocType"] = "CEDULA_EXT";
                                    formContent = new FormUrlEncodedContent(loginFormValues);
                                    using (var qExt = await client.PostAsync(uri, formContent).ConfigureAwait(false))
                                    {
                                        formContent.Dispose();
                                        using (var docExt = await parser.ParseDocumentAsync(await qExt.Content.ReadAsStringAsync().ConfigureAwait(false)).ConfigureAwait(false))
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
                await DownloadAsync(new Uri(urlCaptcha), "solveCaptcha.jpg").ConfigureAwait(false);
                var text = GetText("solveCaptcha.jpg");
                var result = driver
                    .ExecuteAsyncScript($"dwr.engine._execute(ConsultaWeb._path, 'ConsultaWeb', 'verificaDonante', '{rut}', '{text}', function(d){{return d;}})");
                driver.Quit();
                service.Dispose();
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
            using (var send = await client.SendAsync(request).ConfigureAwait(false))
            using (
                Stream contentStream = await (send).Content.ReadAsStreamAsync().ConfigureAwait(false),
                stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None, 3145728, true))
            {
                await contentStream.CopyToAsync(stream).ConfigureAwait(false);
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
            if(rut > 900_000 && rut < 30_000_000 && dv?.Length == 1)
            {
                var fonasa = await _fonasa.GetById(rut).ConfigureAwait(false);
                var array = new string[] { fonasa.ExtNombres, fonasa.ExtApellidoPat, fonasa.ExtApellidoMat };
                var nombre = string.Join(" ", array.Where(s => !string.IsNullOrWhiteSpace(s)));
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    var sii = await GetSII(rut, dv, "RazonSocial").ConfigureAwait(false);
                    if (sii.GetType() == typeof(NotFoundResult))
                    {
                        var servel = await GetNRYF(rut, dv, "name", "rut").ConfigureAwait(false);
                        if (servel.GetType() == typeof(NotFoundResult))
                        {
                            return NotFound();
                        }
                        return Ok(servel);
                    }
                    return Ok(sii);
                }
                return Ok(new { value = nombre });
            }
            return NotFound();
        }

        public static IHtmlCollection<IElement> GetRows(IHtmlDocument doc)
        {
            return doc?.QuerySelectorAll("tr.rowWidth > td");
        }

        public static string GetEstadoCarnet(IHtmlDocument doc)
        {
            return doc?.QuerySelector(".setWidthOfSecondColumn").TextContent;
        }

        public static string Javax(IHtmlDocument doc)
        {
            return doc?.GetElementsByTagName("input").Last().GetAttribute("value");
        }

        public static async Task<IHtmlDocument> GetDoc(Uri rep)
        {
            using (HttpClient hc = new HttpClient())
            {
                var parser = new HtmlParser();
                return await parser.ParseDocumentAsync(await hc.GetStringAsync(rep).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        public static async Task<IHtmlDocument> GetDocStream(Uri rep)
        {
            using (HttpClient hc = new HttpClient())
            {
                var parser = new HtmlParser();
                return await parser.ParseDocumentAsync(await hc.GetStreamAsync(rep).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        public static async Task<HtmlDocument> GetDocXPath(Uri rep)
        {
            using (HttpClient hc = new HttpClient())
            {
                var doc = new HtmlDocument();
                doc.Load(await hc.GetStreamAsync(rep).ConfigureAwait(false));
                return doc;
            }
        }

        public IActionResult ParseName(string full, bool lastfirst)
        {
            var names = full?.Split(" ");
            return lastfirst ?
                Ok(new
                {
                    names = string.Join(" ", names.Skip(2)),
                    psur = names[0],
                    msur = names[1]
                }) :
                Ok(new
                {
                    names = string.Join(" ", names.Take(names.Length - 2).ToArray()),
                    psur = names[names.Length - 1],
                    msur = names[names.Length - 2]
                });
        }
    }
}