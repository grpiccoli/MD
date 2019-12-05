using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using ConsultaMD.Extensions;
using ConsultaMD.Models.Entities;
using ConsultaMD.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ConsultaMD.Controllers
{
    //Médical Insurance Password
    [Authorize]
    public class DrMIPController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFonasa _fonasa;
        public DrMIPController(UserManager<ApplicationUser> userManager,
            IFonasa fonasa)
        {
            _fonasa = fonasa;
            _userManager = userManager;
        }
        [HttpPost]
        [ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetPlaces(int insurance)
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            var run = RUT.Unformat(user.UserName);
            if (run.HasValue)
            {
                var rut = run.Value.rut;
                switch (insurance)
                {
                    case 0:
                        return Ok();
                    case 1:
                        return await Fonasa(rut).ConfigureAwait(false);
                    //Banmédica
                    case 2:
                        //return await Banmedica(rut, pwd).ConfigureAwait(false);
                    //Colmena
                    case 3:
                        //return await Colmena(rut, pwd).ConfigureAwait(false);
                    //Consalud
                    case 4:
                        //return await Consalud(rut, pwd).ConfigureAwait(false);
                    //Cruz Blanca
                    case 5:
                        //return await CruzBlanca(rut, pwd).ConfigureAwait(false);
                    //Nueva Más Vida
                    case 6:
                        //return await NuevaMasvida(rut, pwd).ConfigureAwait(false);
                    //Vida Tres
                    case 7:
                        //return await Vida3(rut, pwd).ConfigureAwait(false);
                    default:
                        return NotFound();
                }
            }
            return NotFound();
        }
        private async Task<IActionResult> Fonasa(int run)
        {
            var fonasa = await _fonasa.GetDocData(run).ConfigureAwait(false);
            if (fonasa != null) return Ok();
            return NotFound();
        }
        private async Task<IActionResult> Banmedica(int rut, string pwd)
        {
            var cookieContainer = new CookieContainer();
            using (HttpClientHandler homeHandler = new HttpClientHandler
            {
                CookieContainer = cookieContainer,
                UseCookies = true
            })
            using (HttpClient homeClient = new HttpClient(homeHandler))
            {
                var homeUri = new Uri("https://www.isaprebanmedica.cl/");
                using (var homeResponse = await homeClient.GetAsync(homeUri).ConfigureAwait(false))
                {
                    using (HttpClientHandler loginHandler = new HttpClientHandler
                    {
                        CookieContainer = cookieContainer,
                        UseCookies = true
                    })
                    {
                        //var test = cookieContainer.GetCookies(homeUri); //
                        using (HttpClient loginClient = new HttpClient(loginHandler))
                        {
                            var loginUri = new Uri("https://www.isaprebanmedica.cl/LoginBanmedica.aspx");
                            using (var loginResponse = await loginClient.GetAsync(loginUri).ConfigureAwait(false))
                            {
                                //var test2 = cookieContainer.GetCookies(homeUri);
                                //var test3 = cookieContainer.GetCookies(loginUri);
                                var parser = new HtmlParser();
                                using (var doc = await parser.ParseDocumentAsync(await loginResponse.Content.ReadAsStringAsync().ConfigureAwait(false)).ConfigureAwait(false))
                                {
                                    var formArray = new Dictionary<string, string>
                                    {
                                        { "__EVENTTARGET", "lnkbtn_login" },
                                        { "__EVENTARGUMENT", "" }
                                    };
                                    formArray = formArray.Concat(doc.GetElementsByTagName("input").Select(i => new
                                    {
                                        name = i.GetAttribute("name"),
                                        value = i.GetAttribute("value")
                                    }).ToDictionary(p => p.name, p => p.value)).ToDictionary(p => p.Key, p => p.Value);
                                    formArray["txt_rut"] = RUT.Format(rut);
                                    formArray["txt_pass"] = pwd;
                                    formArray["ddl_listado"] = "/";

                                    using (FormUrlEncodedContent formContent = new FormUrlEncodedContent(formArray))
                                    using (HttpClientHandler signinHandler = new HttpClientHandler
                                    {
                                        CookieContainer = cookieContainer,
                                        UseCookies = true
                                    })
                                    {
                                        using (HttpClient signinClient = new HttpClient(signinHandler))
                                        {
                                            signinClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3");
                                            signinClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
                                            signinClient.DefaultRequestHeaders.Add("Accept-Lenguage", "es,en-US;q=0.9,en;q=0.8");
                                            signinClient.DefaultRequestHeaders.Add("Cache-Control", "max-age=0");
                                            signinClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
                                            //signinClient.DefaultRequestHeaders.Add("Content-Length", "1381");
                                            //signinClient.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded");
                                            signinClient.DefaultRequestHeaders.Add("Host", "www.isaprebanmedica.cl");
                                            signinClient.DefaultRequestHeaders.Add("Origin", "https://www.isaprebanmedica.cl");
                                            signinClient.DefaultRequestHeaders.Add("Referer", "https://www.isaprebanmedica.cl/LoginBanmedica.aspx");
                                            signinClient.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "navigate");
                                            signinClient.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
                                            signinClient.DefaultRequestHeaders.Add("Sec-Fetch-User", "?1");
                                            signinClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");

                                            using (var signinResponse = await signinClient.PostAsync(loginUri, formContent).ConfigureAwait(false))
                                            {
                                                if (signinResponse.StatusCode == HttpStatusCode.Found)
                                                {
                                                    return Ok();
                                                }
                                            }
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
        private async Task<IActionResult> Colmena(int rut, string pwd)
        {
            var cookieContainer = new CookieContainer();
            using (HttpClientHandler handler = new HttpClientHandler
            {
                CookieContainer = cookieContainer,
                UseCookies = true,
                UseDefaultCredentials = false
            })
            using (HttpClient getclient = new HttpClient(handler))
            {
                var login = new Uri("https://www.colmena.cl/afiliados/");

                using (await getclient.GetAsync(login).ConfigureAwait(false))
                {
                    using (HttpClientHandler setHandler = new HttpClientHandler
                    {
                        CookieContainer = cookieContainer,
                        UseCookies = true,
                        UseDefaultCredentials = false
                    })
                    {
                        var uri = new Uri("https://www.colmena.cl/services/afiliados/authenticate");

                        var loginPayload = new
                        {
                            password = pwd,
                            username = RUT.Format(rut, false, false)
                        };
                        var payloadString = JsonConvert.SerializeObject(loginPayload);
                        using (StringContent httpContent = new StringContent(payloadString, Encoding.UTF8, "application/json"))
                        using (HttpClient client = new HttpClient(setHandler))
                        {
                            client.DefaultRequestHeaders.Add("Referer", "https://www.colmena.cl/afiliados/");
                            using (var response = await client.PostAsync(uri, httpContent).ConfigureAwait(false))
                            {
                                string jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                                var json = JObject.Parse(jsonResponse);
                                string codigo = (string)json.SelectToken("codigoRetorno");
                                if (codigo == "0") return Ok();
                            }
                        }
                    }
                }
            }
            return NotFound();
        }
        private async Task<IActionResult> Consalud(int rut, string pwd)
        {
            var getCookies = new CookieContainer();
            using (HttpClientHandler handler = new HttpClientHandler
            {
                CookieContainer = getCookies,
                UseCookies = true,
                UseDefaultCredentials = false
            })
            using (HttpClient getclient = new HttpClient(handler))
            {
                var login = new Uri("https://clientes.consalud.cl/Default.aspx");

                using (await getclient.GetAsync(login).ConfigureAwait(false))
                {
                    var loginCookie = getCookies.GetCookies(login).Cast<Cookie>().FirstOrDefault();

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
                            { "usr", RUT.Format(rut,false) },
                            { "pwd", pwd },
                            { "ingresar", "Ingresar" }
                        };
                        using (FormUrlEncodedContent formContent = new FormUrlEncodedContent(loginFormValues))
                        using (HttpClient client = new HttpClient(handler))
                        {
                            client.DefaultRequestHeaders.Add("Referer", "https://clientes.consalud.cl/Default.aspx");
                            using (var response = await client.PostAsync(uri, formContent).ConfigureAwait(false))
                            {
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
        private async Task<IActionResult> CruzBlanca(int rut, string pwd)
        {
            var getCookies = new CookieContainer();
            using (HttpClientHandler handler = new HttpClientHandler
            {
                CookieContainer = getCookies,
                UseCookies = true,
                UseDefaultCredentials = false
            })
            using (HttpClient getclient = new HttpClient(handler))
            {
                var login = new Uri("https://clientes.consalud.cl/Default.aspx");

                using (await getclient.GetAsync(login).ConfigureAwait(false))
                {
                    var loginCookie = getCookies.GetCookies(login).Cast<Cookie>().FirstOrDefault();

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
                            { "usr", RUT.Format(rut,false) },
                            { "pwd", pwd },
                            { "ingresar", "Ingresar" }
                        };
                        using (FormUrlEncodedContent formContent = new FormUrlEncodedContent(loginFormValues))
                        using (HttpClient client = new HttpClient(handler))
                        {
                            client.DefaultRequestHeaders.Add("Referer", "https://clientes.consalud.cl/Default.aspx");
                            using (var response = await client.PostAsync(uri, formContent).ConfigureAwait(false))
                            {
                                var parser = new HtmlParser();
                                using (var doc = await parser.ParseDocumentAsync(await response.Content.ReadAsStringAsync().ConfigureAwait(false)).ConfigureAwait(false))
                                {
                                    var test = doc.GetElementById("~~~");
                                    if (test != null)
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
        private async Task<IActionResult> NuevaMasvida(int rut, string pwd)
        {
            var cookieContainer = new CookieContainer();
            using (HttpClientHandler handler = new HttpClientHandler
            {
                CookieContainer = cookieContainer,
                UseCookies = true
            })
            using (HttpClient getclient = new HttpClient(handler))
            {
                var loginString = "https://sv.nuevamasvida.cl/index.php";
                var login = new Uri(loginString);

                using (await getclient.GetAsync(login).ConfigureAwait(false))
                {
                    using (HttpClientHandler setHandler = new HttpClientHandler
                    {
                        CookieContainer = cookieContainer,
                        UseCookies = true
                    })
                    {
                        var uri = new Uri("https://sv.nuevamasvida.cl/sucursal_virtual/");

                        var loginFormValues = new Dictionary<string, string>
                        {
                            { "pag", "" },
                            { "rut", RUT.Format(rut,false) },
                            { "clave", pwd }
                        };
                        using (FormUrlEncodedContent formContent = new FormUrlEncodedContent(loginFormValues))
                        using (HttpClient client = new HttpClient(handler))
                        {
                            client.DefaultRequestHeaders.Add("Referer", loginString);
                            //client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "navigate");
                            //client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
                            //client.DefaultRequestHeaders.Add("Sec-Fetch-User", "?1");
                            using (var response = await client.PostAsync(uri, formContent).ConfigureAwait(false))
                            {
                                if (response.RequestMessage.RequestUri.ToString() == "https://sv.nuevamasvida.cl/sucursal_virtual/")
                                {
                                    return Ok();
                                }
                            }
                        }
                    }
                }
            }

            return NotFound();
        }
        private async Task<IActionResult> Vida3(int rut, string pwd)
        {
            var getCookies = new CookieContainer();
            using (HttpClientHandler handler = new HttpClientHandler
            {
                CookieContainer = getCookies,
                UseCookies = true,
                UseDefaultCredentials = false
            })
            using (HttpClient getclient = new HttpClient(handler))
            {
                var login = new Uri("https://clientes.consalud.cl/Default.aspx");

                using (await getclient.GetAsync(login).ConfigureAwait(false))
                {
                    var loginCookie = getCookies.GetCookies(login).Cast<Cookie>().FirstOrDefault();

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
                            { "usr", RUT.Format(rut,false) },
                            { "pwd", pwd },
                            { "ingresar", "Ingresar" }
                        };
                        using (FormUrlEncodedContent formContent = new FormUrlEncodedContent(loginFormValues))
                        using (HttpClient client = new HttpClient(handler))
                        {
                            client.DefaultRequestHeaders.Add("Referer", "https://clientes.consalud.cl/Default.aspx");
                            using (var response = await client.PostAsync(uri, formContent).ConfigureAwait(false))
                            {
                                var parser = new HtmlParser();
                                using (var doc = await parser.ParseDocumentAsync(await response.Content.ReadAsStringAsync().ConfigureAwait(false)).ConfigureAwait(false))
                                {
                                    var test = doc.GetElementById("~~~");
                                    if (test != null)
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
    }
}