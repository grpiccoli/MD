﻿using AngleSharp.Html.Parser;
using ConsultaMD.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ConsultaMD.Services
{
    public sealed class MIPService : IMIP
    {
        private readonly IFonasa _fonasa;

        public MIPService(IFonasa fonasa) {
            _fonasa = fonasa;
        }
        public async Task<bool> Validate(int insurance, int rut, string pwd) {
            return insurance switch
            {
                0 => true,
                1 => await Fonasa(rut).ConfigureAwait(false),
                2 => await Banmedica(rut, pwd).ConfigureAwait(false),
                3 => await Colmena(rut, pwd).ConfigureAwait(false),
                4 => await Consalud(rut, pwd).ConfigureAwait(false),
                5 => await CruzBlanca(rut, pwd).ConfigureAwait(false),
                6 => await NuevaMasvida(rut, pwd).ConfigureAwait(false),
                7 => await Vida3(rut, pwd).ConfigureAwait(false),
                _ => false
            };
        }
        private async Task<bool> Fonasa(int run)
        {
            var fonasa = await _fonasa.GetByIdAsync(run).ConfigureAwait(false);
            if (!string.IsNullOrWhiteSpace(fonasa?.ExtGrupoIng)) return true;
            return false;
        }
        private static async Task<bool> Banmedica(int rut, string pwd)
        {
            var cookieContainer = new CookieContainer();
            var homeUri = new Uri("https://www.isaprebanmedica.cl/");
            var loginUri = new Uri("https://www.isaprebanmedica.cl/LoginBanmedica.aspx");
            var parser = new HtmlParser();
            using (HttpClientHandler homeHandler = new HttpClientHandler
            {
                CookieContainer = cookieContainer,
                UseCookies = true
            })
            using (HttpClient homeClient = new HttpClient(homeHandler))
            using (var homeResponse = await homeClient.GetAsync(homeUri).ConfigureAwait(false))
            using (HttpClientHandler loginHandler = new HttpClientHandler
            {
                CookieContainer = cookieContainer,
                UseCookies = true
            })
            //var test = cookieContainer.GetCookies(homeUri); //
            using (HttpClient loginClient = new HttpClient(loginHandler))
            using (var loginResponse = await loginClient.GetAsync(loginUri).ConfigureAwait(false))
            //var test2 = cookieContainer.GetCookies(homeUri);
            //var test3 = cookieContainer.GetCookies(loginUri);
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

                using FormUrlEncodedContent formContent = new FormUrlEncodedContent(formArray);
                using HttpClientHandler signinHandler = new HttpClientHandler
                {
                    CookieContainer = cookieContainer,
                    UseCookies = true
                };
                using HttpClient signinClient = new HttpClient(signinHandler);
                signinClient.DefaultRequestHeaders.Add("Accept",
"text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3");
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

                using var signinResponse = await signinClient.PostAsync(loginUri, formContent).ConfigureAwait(false);
                if (signinResponse.StatusCode == HttpStatusCode.Found)
                {
                    return true;
                }
            }
            return false;
        }
        private static async Task<bool> Colmena(int rut, string pwd)
        {
            var cookieContainer = new CookieContainer();
            var login = new Uri("https://www.colmena.cl/afiliados/");
            var uri = new Uri("https://www.colmena.cl/services/afiliados/authenticate");

            var loginPayload = new
            {
                password = pwd,
                username = RUT.Format(rut, false, false)
            };
            var payloadString = JsonConvert.SerializeObject(loginPayload);
            using (HttpClientHandler handler = new HttpClientHandler
            {
                CookieContainer = cookieContainer,
                UseCookies = true,
                UseDefaultCredentials = false
            })
            using (HttpClient getclient = new HttpClient(handler))
            using (await getclient.GetAsync(login).ConfigureAwait(false))
            using (HttpClientHandler setHandler = new HttpClientHandler
            {
                CookieContainer = cookieContainer,
                UseCookies = true,
                UseDefaultCredentials = false
            })
            using (StringContent httpContent = new StringContent(payloadString, Encoding.UTF8, "application/json"))
            using (HttpClient client = new HttpClient(setHandler))
            {
                client.DefaultRequestHeaders.Add("Referer", "https://www.colmena.cl/afiliados/");
                using var response = await client.PostAsync(uri, httpContent).ConfigureAwait(false);
                string jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var json = JObject.Parse(jsonResponse);
                string codigo = (string)json.SelectToken("codigoRetorno");
                if (codigo == "0") return true;
            }
            return false;
        }
        private static async Task<bool> Consalud(int rut, string pwd)
        {
            var getCookies = new CookieContainer();
            var login = new Uri("https://clientes.consalud.cl/Default.aspx");
            var uri = new Uri("https://clientes.consalud.cl/auw.aspx");
            var loginFormValues = new Dictionary<string, string>
                        {
                            { "esWeb", "1" },
                            { "usr", RUT.Format(rut,false) },
                            { "pwd", pwd },
                            { "ingresar", "Ingresar" }
                        };
            var parser = new HtmlParser();
            using (HttpClientHandler handler = new HttpClientHandler
            {
                CookieContainer = getCookies,
                UseCookies = true,
                UseDefaultCredentials = false
            })
            using (HttpClient getclient = new HttpClient(handler))
            using (await getclient.GetAsync(login).ConfigureAwait(false))
            using (HttpClientHandler setHandler = new HttpClientHandler
            {
                CookieContainer = getCookies,
                UseCookies = true,
                UseDefaultCredentials = false
            })
            using (FormUrlEncodedContent formContent = new FormUrlEncodedContent(loginFormValues))
            using (HttpClient client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Add("Referer", "https://clientes.consalud.cl/Default.aspx");
                using var response = await client.PostAsync(uri, formContent).ConfigureAwait(false);
                using var doc = await parser.ParseDocumentAsync(await response.Content
                    .ReadAsStringAsync().ConfigureAwait(false)).ConfigureAwait(false);
                var test = doc.GetElementById("esWeb");
                if (test == null)
                {
                    return true;
                }
            }
            return false;
        }
        private static async Task<bool> CruzBlanca(int rut, string pwd)
        {
            var getCookies = new CookieContainer();
            var login = new Uri("https://clientes.consalud.cl/Default.aspx");
            var uri = new Uri("https://clientes.consalud.cl/auw.aspx");
            var loginFormValues = new Dictionary<string, string>
                        {
                            { "esWeb", "1" },
                            { "usr", RUT.Format(rut,false) },
                            { "pwd", pwd },
                            { "ingresar", "Ingresar" }
                        };
            var parser = new HtmlParser();
            using (HttpClientHandler handler = new HttpClientHandler
            {
                CookieContainer = getCookies,
                UseCookies = true,
                UseDefaultCredentials = false
            })
            using (HttpClient getclient = new HttpClient(handler))
            using (await getclient.GetAsync(login).ConfigureAwait(false))
            using (HttpClientHandler setHandler = new HttpClientHandler
            {
                CookieContainer = getCookies,
                UseCookies = true,
                UseDefaultCredentials = false
            })
            using (FormUrlEncodedContent formContent = new FormUrlEncodedContent(loginFormValues))
            using (HttpClient client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Add("Referer", "https://clientes.consalud.cl/Default.aspx");
                using var response = await client.PostAsync(uri, formContent).ConfigureAwait(false);
                using var doc = await parser.ParseDocumentAsync(await response.Content
                    .ReadAsStringAsync().ConfigureAwait(false)).ConfigureAwait(false);
                var test = doc.GetElementById("~~~");
                if (test != null)
                {
                    return true;
                }
            }
            return false;
        }
        private static async Task<bool> NuevaMasvida(int rut, string pwd)
        {
            var cookieContainer = new CookieContainer();
            var loginString = "https://sv.nuevamasvida.cl/index.php";
            var login = new Uri(loginString);
            var uri = new Uri("https://sv.nuevamasvida.cl/sucursal_virtual/");
            var loginFormValues = new Dictionary<string, string>
                        {
                            { "pag", "" },
                            { "rut", RUT.Format(rut,false) },
                            { "clave", pwd }
                        };
            using (HttpClientHandler handler = new HttpClientHandler
            {
                CookieContainer = cookieContainer,
                UseCookies = true
            })
            using (HttpClient getclient = new HttpClient(handler))
            using (await getclient.GetAsync(login).ConfigureAwait(false))
            using (HttpClientHandler setHandler = new HttpClientHandler
            {
                CookieContainer = cookieContainer,
                UseCookies = true
            })
            using (FormUrlEncodedContent formContent = new FormUrlEncodedContent(loginFormValues))
            using (HttpClient client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Add("Referer", loginString);
                //client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "navigate");
                //client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
                //client.DefaultRequestHeaders.Add("Sec-Fetch-User", "?1");
                using var response = await client.PostAsync(uri, formContent).ConfigureAwait(false);
                if (response.RequestMessage.RequestUri.ToString() == "https://sv.nuevamasvida.cl/sucursal_virtual/")
                {
                    return true;
                }
            }
            return false;
        }
        private static async Task<bool> Vida3(int rut, string pwd)
        {
            var getCookies = new CookieContainer();
            var get = new Uri("https://www.isaprevidatres.cl/LoginVidaTres.aspx");
            var post = new Uri("https://clientes.consalud.cl/auw.aspx");
            var loginFormValues = new Dictionary<string, string>
                {
                    { "esWeb", "1" },
                    { "usr", RUT.Format(rut,false) },
                    { "pwd", pwd },
                    { "ingresar", "Ingresar" }
                };
            var parser = new HtmlParser();
            using (HttpClientHandler getHandler = new HttpClientHandler
            {
                CookieContainer = getCookies,
                UseCookies = true,
                UseDefaultCredentials = false
            })
            using (HttpClient getclient = new HttpClient(getHandler))
            using (await getclient.GetAsync(get).ConfigureAwait(false))
            using (HttpClientHandler postHandler = new HttpClientHandler
            {
                CookieContainer = getCookies,
                UseCookies = true,
                UseDefaultCredentials = false
            })
            using (FormUrlEncodedContent formContent = new FormUrlEncodedContent(loginFormValues))
            using (HttpClient client = new HttpClient(postHandler))
            {
                client.DefaultRequestHeaders.Add("Referer", "https://clientes.consalud.cl/Default.aspx");
                using var response = await client.PostAsync(post, formContent).ConfigureAwait(false);
                using var doc = await parser.ParseDocumentAsync(await response.Content
                    .ReadAsStringAsync().ConfigureAwait(false)).ConfigureAwait(false);
                var test = doc.GetElementById("~~~");
                if (test != null)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
