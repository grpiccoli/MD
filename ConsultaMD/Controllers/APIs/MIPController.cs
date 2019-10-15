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
    public class MIPController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public MIPController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        [HttpPost]
        [ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Validate(int insurance, string pwd)
        {
            var user = await _userManager.GetUserAsync(User);
            var run = RUT.Unformat(user.UserName);
            var rut = run.Value.rut;
            var dv = run.Value.dv;
            switch (insurance)
            {
                case 0:
                    return Ok();
                case 1:
                    return await Fonasa(rut, dv);
                //Banmédica
                case 2:
                    return await Banmedica(rut, dv, pwd);
                //Colmena
                case 3:
                    return await Colmena(rut, dv, pwd);
                //Consalud
                case 4:
                    return await Consalud(rut, dv, pwd);
                //Cruz Blanca
                case 5:
                    return await CruzBlanca(rut, dv, pwd);
                //Nueva Más Vida
                case 6:
                    return await NuevaMasVida(rut, dv, pwd);
                //Vida Tres
                case 7:
                    return await Vida3(rut, dv, pwd);
                default:
                    return NotFound();
            }
        }
        [HttpPost]
        [ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Fonasa(int rut, string dv)
        {
            var getCookies = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler
            {
                CookieContainer = getCookies,
                UseCookies = true,
                UseDefaultCredentials = false
            };
            using (HttpClient getclient = new HttpClient(handler))
            {
                var login = new Uri("https://bonowebfon.fonasa.cl/");
                using (var response = await getclient.GetAsync(login))
                {
                    var parser = new HtmlParser();
                    using (var doc = await parser.ParseDocumentAsync(await response.Content.ReadAsStringAsync()))
                    {
                        var formArray = doc.GetElementsByTagName("input").Select(i => new {
                            name = i.GetAttribute("name"),
                            value = i.GetAttribute("value")
                        }).ToDictionary(p => p.name, p => p.value);
                        formArray["RutBeneficiario"] = $"00{rut}-{dv}";
                        formArray["captcha_code"] = "XXXXX";

                        var formContent = new FormUrlEncodedContent(formArray);

                        using (HttpClient client = new HttpClient())
                        {
                            using (var logResponse = await client.PostAsync(login, formContent))
                            {
                                using (var docResponse = await parser.ParseDocumentAsync(await logResponse.Content.ReadAsStringAsync()))
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
        [HttpPost]
        [ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Banmedica(int rut, string dv, string pwd)
        {
            var cookieContainer = new CookieContainer();
            HttpClientHandler homeHandler = new HttpClientHandler
            {
                CookieContainer = cookieContainer,
                UseCookies = true
            };

            using (HttpClient homeClient = new HttpClient(homeHandler))
            {
                var homeUri = new Uri("https://www.isaprebanmedica.cl/");
                using (var homeResponse = await homeClient.GetAsync(homeUri))
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
                            using (var loginResponse = await loginClient.GetAsync(loginUri))
                            {
                                //var test2 = cookieContainer.GetCookies(homeUri);
                                //var test3 = cookieContainer.GetCookies(loginUri);
                                var parser = new HtmlParser();
                                using (var doc = await parser.ParseDocumentAsync(await loginResponse.Content.ReadAsStringAsync()))
                                {
                                    var formArray = new Dictionary<string, string>
                                    {
                                        { "__EVENTTARGET", "lnkbtn_login" },
                                        { "__EVENTARGUMENT", "" }
                                    };
                                    formArray = formArray.Concat(doc.GetElementsByTagName("input").Select(i => new {
                                        name = i.GetAttribute("name"),
                                        value = i.GetAttribute("value")
                                    }).ToDictionary(p => p.name, p => p.value)).ToDictionary(p => p.Key, p => p.Value);
                                    formArray["txt_rut"] = RUT.Format(rut);
                                    formArray["txt_pass"] = pwd;
                                    formArray["ddl_listado"] = "/";

                                    var formContent = new FormUrlEncodedContent(formArray);

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

                                            using (var signinResponse = await signinClient.PostAsync(loginUri, formContent))
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
        [HttpPost]
        [ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Colmena(int rut, string dv, string pwd)
        {
            var cookieContainer = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler
            {
                CookieContainer = cookieContainer,
                UseCookies = true,
                UseDefaultCredentials = false
            };
            using (HttpClient getclient = new HttpClient(handler))
            {
                var login = new Uri("https://www.colmena.cl/afiliados/");

                using (await getclient.GetAsync(login))
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
                            username = $"{rut}{dv}"
                        };
                        var payloadString = JsonConvert.SerializeObject(loginPayload);
                        var httpContent = new StringContent(payloadString, Encoding.UTF8, "application/json");
                        using (HttpClient client = new HttpClient(setHandler))
                        {
                            client.DefaultRequestHeaders.Add("Referer", "https://www.colmena.cl/afiliados/");
                            using (var response = await client.PostAsync(uri, httpContent))
                            {
                                string jsonResponse = await response.Content.ReadAsStringAsync();
                                var json = JObject.Parse(jsonResponse);
                                string codigo = (string)json.SelectToken("codigoRetorno");
                                if(codigo == "0") return Ok();
                            }
                        }
                    }
                }
            }
            return NotFound();
        }
        [HttpPost]
        [ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Consalud(int rut, string dv, string pwd)
        {
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

                using (await getclient.GetAsync(login))
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
                            { "usr", $"{rut}-{dv}" },
                            { "pwd", pwd },
                            { "ingresar", "Ingresar" }
                        };
                        var formContent = new FormUrlEncodedContent(loginFormValues);

                        using (HttpClient client = new HttpClient(handler))
                        {
                            client.DefaultRequestHeaders.Add("Referer", "https://clientes.consalud.cl/Default.aspx");
                            using (var response = await client.PostAsync(uri, formContent))
                            {
                                var parser = new HtmlParser();
                                using (var doc = await parser.ParseDocumentAsync(await response.Content.ReadAsStringAsync()))
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
        [HttpPost]
        [ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CruzBlanca(int rut, string dv, string pwd)
        {
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

                using (await getclient.GetAsync(login))
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
                            { "usr", $"{rut}-{dv}" },
                            { "pwd", pwd },
                            { "ingresar", "Ingresar" }
                        };
                        var formContent = new FormUrlEncodedContent(loginFormValues);

                        using (HttpClient client = new HttpClient(handler))
                        {
                            client.DefaultRequestHeaders.Add("Referer", "https://clientes.consalud.cl/Default.aspx");
                            using (var response = await client.PostAsync(uri, formContent))
                            {
                                var parser = new HtmlParser();
                                using (var doc = await parser.ParseDocumentAsync(await response.Content.ReadAsStringAsync()))
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
        [HttpPost]
        [ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> NuevaMasVida(int rut, string dv, string pwd)
        {
            var cookieContainer = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler
            {
                CookieContainer = cookieContainer,
                UseCookies = true
            };

            using (HttpClient getclient = new HttpClient(handler))
            {
                var login = new Uri("https://sv.nuevamasvida.cl/index.php");

                using (await getclient.GetAsync(login))
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
                            { "rut", $"{rut}-{dv}" },
                            { "clave", pwd }
                        };
                        var formContent = new FormUrlEncodedContent(loginFormValues);

                        using (HttpClient client = new HttpClient(handler))
                        {
                            client.DefaultRequestHeaders.Add("Referer", "https://sv.nuevamasvida.cl/index.php");
                            using (var response = await client.PostAsync(uri, formContent))
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
        [HttpPost]
        [ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Vida3(int rut, string dv, string pwd)
        {
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

                using (await getclient.GetAsync(login))
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
                            { "usr", $"{rut}-{dv}" },
                            { "pwd", pwd },
                            { "ingresar", "Ingresar" }
                        };
                        var formContent = new FormUrlEncodedContent(loginFormValues);

                        using (HttpClient client = new HttpClient(handler))
                        {
                            client.DefaultRequestHeaders.Add("Referer", "https://clientes.consalud.cl/Default.aspx");
                            using (var response = await client.PostAsync(uri, formContent))
                            {
                                var parser = new HtmlParser();
                                using (var doc = await parser.ParseDocumentAsync(await response.Content.ReadAsStringAsync()))
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