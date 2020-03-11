using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OpenQA.Selenium.Chrome;
using Tesseract;

namespace ConsultaMD.Controllers
{
    [Authorize]
    [Produces("application/json")]
    //Servicios Públicos
    public class SPController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IFonasa _fonasa;
        private readonly IRegCivil _regCivil;
        public SPController(
            IWebHostEnvironment environment,
            IRegCivil regCivil,
            IFonasa fonasa)
        {
            _regCivil = regCivil;
            _fonasa = fonasa;
            _environment = environment;
        }
        [HttpPost]
        [ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //val  : name| rut | sex | address | commune | all
        //type : rut | buscar
        public async Task<IActionResult> GetNRYF(int rut, string val, string type)
        {
            var rutF = RUT.Format(rut);
            var loginFormValues = new Dictionary<string, string>
            {
                { "term", rutF }
            };
            var uri = new Uri($"https://www.nombrerutyfirma.com/{type}");
            CookieContainer getCookies = new CookieContainer();
            using FormUrlEncodedContent formContent = new FormUrlEncodedContent(loginFormValues);
            using HttpClientHandler getHandler = new HttpClientHandler
            {
                CookieContainer = getCookies,
                UseCookies = true,
                UseDefaultCredentials = false
            };
            using HttpClient client = new HttpClient(getHandler);
            using var response = await client.PostAsync(uri, formContent).ConfigureAwait(false);
            var parser = new HtmlParser();
            using var doc = await parser.ParseDocumentAsync(await response.Content.ReadAsStringAsync().ConfigureAwait(false)).ConfigureAwait(false);
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
        [HttpPost]
        [ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //val : rut | razon_social | actividades | all
        public IActionResult GetSII(int run, string dv, string val)
        {
            if (RUT.IsValid(run, dv))
            {
                var rut = RUT.Format(run);
                var script = Path.Combine(_environment.ContentRootPath, "src", "scripts", "node", "SII.js");
                using var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "C:\\Program Files\\nodejs\\node.exe",
                        Arguments = $"{script} {rut}",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    }
                };
                var s = string.Empty;
                var e = string.Empty;
                process.OutputDataReceived += (sender, data) => s += data.Data;
                process.ErrorDataReceived += (sender, data) => e += data.Data;
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
                process.Close();
                var siiData = JsonConvert.DeserializeObject<SII>(s, 
                    new JsonSerializerSettings 
                    {
                        ContractResolver = new DefaultContractResolver 
                        { 
                            NamingStrategy = new SnakeCaseNamingStrategy() 
                        }
                    });
                if (!string.IsNullOrEmpty(e) || string.IsNullOrEmpty(siiData.RazonSocial)) return NotFound();
                return val == "all" ? Ok(siiData) : Ok(siiData[val]);
            }
            return NotFound();
        }
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DocumentRequestStatus(int rut, string dv, int carnet, bool ext)
        {
            if (rut > 900_000 && rut < 30_000_000 && RUT.IsValid(rut,dv) && carnet > 100_000_000 && carnet < 999_999_999)
            {
                var result = await _regCivil.IsValidAsync(rut, carnet, ext).ConfigureAwait(false);
                return Ok(result);
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

            var Options = new ChromeOptions();
            //Options.AddArguments("headless", "disable-gpu");
            using ChromeDriverService service = ChromeDriverService.CreateDefaultService(_environment.ContentRootPath);
            using var driver = new ChromeDriver(service, Options);
            driver.Navigate().GoToUrl(uri);
            var urlCaptcha = driver.FindElementById("simplecaptchaimg").GetAttribute("src");
            await DownloadAsync(new Uri(urlCaptcha), "solveCaptcha.jpg").ConfigureAwait(false);
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
        public static async Task DownloadAsync(Uri requestUri, string filename)
        {
            using var client = new HttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            using var send = await client.SendAsync(request).ConfigureAwait(false);
            using Stream contentStream = await (send).Content.ReadAsStreamAsync().ConfigureAwait(false),
stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None, 3145728, true);
            await contentStream.CopyToAsync(stream).ConfigureAwait(false);
        }
        public static string GetText(string imgsource)
        {
            var ocrtext = string.Empty;
            using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
            {
                using var img = Pix.LoadFromFile(imgsource);
                using var page = engine.Process(img);
                ocrtext = page.GetText();
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
                var fonasa = await _fonasa.GetByIdAsync(rut).ConfigureAwait(false);
                var array = new string[] { fonasa.ExtNombres, fonasa.ExtApellidoPat, fonasa.ExtApellidoMat };
                var nombre = string.Join(" ", array.Where(s => !string.IsNullOrWhiteSpace(s)));
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    var sii = GetSII(rut, dv, "RazonSocial");
                    if (sii.GetType() == typeof(NotFoundResult))
                    {
                        var servel = await GetNRYF(rut, "name", "rut").ConfigureAwait(false);
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

        public static async Task<IHtmlDocument> GetDoc(Uri rep)
        {
            using HttpClient hc = new HttpClient();
            var parser = new HtmlParser();
            return await parser.ParseDocumentAsync(await hc.GetStringAsync(rep).ConfigureAwait(false)).ConfigureAwait(false);
        }

        public static async Task<IHtmlDocument> GetDocStream(Uri rep)
        {
            using HttpClient hc = new HttpClient();
            var parser = new HtmlParser();
            return await parser.ParseDocumentAsync(await hc.GetStreamAsync(rep).ConfigureAwait(false)).ConfigureAwait(false);
        }

        public static async Task<HtmlDocument> GetDocXPath(Uri rep)
        {
            using HttpClient hc = new HttpClient();
            var doc = new HtmlDocument();
            doc.Load(await hc.GetStreamAsync(rep).ConfigureAwait(false));
            return doc;
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
                    psur = names[^1],
                    msur = names[^2]
                });
        }
    }
}