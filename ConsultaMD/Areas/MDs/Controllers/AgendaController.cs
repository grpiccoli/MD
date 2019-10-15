﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using ConsultaMD.Areas.MDs.Models;
using ConsultaMD.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ConsultaMD.Areas.MDs.Views
{
    [Area("MDs")]
    public class AgendaController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly string _prefix;
        private readonly Uri _loginUri;
        private readonly Uri _dataFeed;
        private readonly Uri _calendar;
        private readonly Uri _doctores;
        private readonly Uri _interfaces;
        private readonly Uri _schedule;
        private readonly Uri _selectDr;
        private readonly Uri _getEvents;
        public string os = Environment.OSVersion.Platform.ToString();

        public AgendaController(
            UserManager<ApplicationUser> userManager)
        {
            //_prefix = "http://valhalla.backupcode.net:8091";
            _prefix = "http://consultas.oftalmontt.cl:81";
            //_prefix = "http://192.168.0.254";
            _interfaces = new Uri($"{_prefix}/interfaces");
            _schedule = new Uri($"{_interfaces}/patientSchedule");
            _loginUri = new Uri($"{_interfaces}/login/control.php");
            _calendar = new Uri($"{_schedule}/menuV1.php");
            _doctores = new Uri($"{_schedule}/carga_loadselect.php");
            _selectDr = new Uri($"{_schedule}/carga_idproffesional.php");
            _dataFeed = new Uri($"{_prefix}/class/php/patient/datafeed.php");
            _getEvents = new Uri($"{_dataFeed}?method=list");
            _userManager = userManager;
        }

        public string[] colors = new string[]
{
            "#FFFFFF","#808080","#000000","#FF0000","#800000","#FFFF00",
            "#808000","#00FF00","#008000","#00FFFF","#008080","#0000FF",
            "#000080","#FF00FF","#800080"
};

        public Dictionary<string, string> places = new Dictionary<string, string>()
        {
            { "Oftalmontt Puerto Montt", "Clínica Pto Montt" },
            { "Oftalmontt Puerto Varas", "Clínica Oftalmontt Pto Varas"},
            { "Consulta HMenares", "Ed. Baquedano of. Dr. Menares"},
            { "Consulta MParis", "Ed. Baquedano of. Dr. Paris"},
            { "Consulta GNEUMANN", "Ed. Baquedano of. Dr. Neumann"},
            { "Consulta JROSAS", "Consulta Dr. Rosas"},
            { "Consulta CLINICA UNIVERSITARIA", "Clínica Universitaria"}
        };

        public string[] formats = new string[]
        {
            "ddd MMM dd yyyy HH:mm:ss 'GMT'K '(Chile Summer Time)'",
            "ddd MMM dd yyyy HH:mm:ss 'GMT'K '(Chile Standard Time)'",
            "ddd MMM dd yyyy HH:mm:ss 'GMT'K '(hora estándar de Chile)'",
            "MM/dd/yyyy HH:mm"
        };

        public async Task<IActionResult> Index(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            var loginFormValues = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("username", "16587043"),
                new KeyValuePair<string, string>("password", "admin"),
                new KeyValuePair<string, string>("roles_usuario", "1"),
                new KeyValuePair<string, string>("location", "1")
            };
            var formContent = new FormUrlEncodedContent(loginFormValues);

            CookieContainer getCookies = new CookieContainer();
            HttpClientHandler getHandler = new HttpClientHandler
            {
                CookieContainer = getCookies,
                UseCookies = true,
                UseDefaultCredentials = false
            };

            using (HttpClient client = new HttpClient(getHandler)) {
                var parser = new HtmlParser();
                var doc = await parser.ParseDocumentAsync(await client.GetStringAsync(_loginUri));
                var locationOptions = doc.QuerySelectorAll("#location > option")
                    .Select(o => new Location
                    {
                        Value = o.GetAttribute("value"),
                        Text = places.ContainsKey(o.InnerHtml) ? places[o.InnerHtml] : o.InnerHtml
                    }).ToList();
                await client.PostAsync(_loginUri, formContent);
                var loginCookie = getCookies.GetCookies(_loginUri).Cast<System.Net.Cookie>().FirstOrDefault();
                var setCookies = new CookieContainer();
                setCookies.Add(_calendar, loginCookie);
                HttpClientHandler setHandler = new HttpClientHandler
                {
                    CookieContainer = setCookies,
                    UseCookies = true,
                    UseDefaultCredentials = false
                };
                using (HttpClient hc = new HttpClient(setHandler))
                {
                    var parser2 = new HtmlParser();
                    var doc2 = await parser2.ParseDocumentAsync(await hc.GetStringAsync(_calendar));
                    var doctores = doc2.QuerySelectorAll("#nameProfesional option")
                        .Select(o => new DoctorSelect { Value = o.GetAttribute("value"), Text = o.InnerHtml }).ToList();
                    var model = new AgendaVM
                    {
                        LocationList = locationOptions,
                        DoctorList = doctores,
                        ColorList = colors
                    };
                    if (id.HasValue) model.DoctorId = id.ToString();
                    return View(model);
                }
            }
        }

        public async Task<IActionResult> GetEvents(int id, string start)
        {
            var citas = new HashSet<Cita>();

            if (ModelState.IsValid && id != 0)
            {
                var user = await _userManager.GetUserAsync(User);
                var loginFormValues = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("username", "16587043"),
                    new KeyValuePair<string, string>("password", "admin"),
                    new KeyValuePair<string, string>("roles_usuario", "1"),
                    new KeyValuePair<string, string>("location", "1")
                };
                using (var formContent = new FormUrlEncodedContent(loginFormValues))
                {
                    CookieContainer getCookies = new CookieContainer();
                    HttpClientHandler getHandler = new HttpClientHandler
                    {
                        CookieContainer = getCookies,
                        UseCookies = true,
                        UseDefaultCredentials = false
                    };

                    using (HttpClient client = new HttpClient(getHandler))
                    {
                        var parser = new HtmlParser();
                        var doc = await parser.ParseDocumentAsync(await client.GetStringAsync(_loginUri));
                        var locationOptions = doc.QuerySelectorAll("#location > option")
                            .Select(o => new Location { Value = o.GetAttribute("value"), Text = o.InnerHtml }).ToList();

                        var tasks = locationOptions
                            .Select(
                            async loc =>
                        //foreach (var loc in locationOptions)
                        {
                                var locForm = new List<KeyValuePair<string, string>>
                                {
                            new KeyValuePair<string, string>("username", "16587043"),
                            new KeyValuePair<string, string>("password", "admin"),
                            new KeyValuePair<string, string>("roles_usuario", "1"),
                            new KeyValuePair<string, string>("location", loc.Value)
                                };
                                var locContent = new FormUrlEncodedContent(locForm);

                                CookieContainer locCookies = new CookieContainer();
                                HttpClientHandler locHandler = new HttpClientHandler
                                {
                                    CookieContainer = locCookies,
                                    UseCookies = true,
                                    UseDefaultCredentials = false
                                };

                                HttpClient locClient = new HttpClient(locHandler);
                                var locPos = await locClient.PostAsync(_loginUri, locContent);
                                var locRes = await locPos.Content.ReadAsStringAsync();

                                System.Net.Cookie locCookie = locCookies.GetCookies(_loginUri).Cast<System.Net.Cookie>().FirstOrDefault();

                                CookieContainer docCookies = new CookieContainer();
                                docCookies.Add(_doctores, locCookie);
                                HttpClientHandler docHandler = new HttpClientHandler
                                {
                                    CookieContainer = docCookies,
                                    UseCookies = true,
                                    UseDefaultCredentials = false
                                };
                                HttpClient docClient = new HttpClient(docHandler);
                                var tmp = await docClient.GetStringAsync(_doctores);

                                if (tmp.Contains("=" + id.ToString() + ">"))
                                {
                                    System.Net.Cookie docCookie = docCookies.GetCookies(_doctores).Cast<System.Net.Cookie>().FirstOrDefault();

                                    CookieContainer selCookies = new CookieContainer();
                                    selCookies.Add(_selectDr, docCookie);
                                    HttpClientHandler selHandler = new HttpClientHandler
                                    {
                                        CookieContainer = selCookies,
                                        UseCookies = true,
                                        UseDefaultCredentials = false
                                    };
                                    HttpClient selClient = new HttpClient(selHandler);
                                    var selForm = new List<KeyValuePair<string, string>>
                                    {
                                new KeyValuePair<string, string>("idp", id.ToString())
                                    };
                                    var selContent = new FormUrlEncodedContent(selForm);
                                    var selPos = await selClient.PostAsync(_selectDr, selContent);

                                    var selRes = await selPos.Content.ReadAsStringAsync();

                                    System.Net.Cookie selCookie = selCookies.GetCookies(_selectDr).Cast<System.Net.Cookie>().FirstOrDefault();

                                    CookieContainer eventCookies = new CookieContainer();
                                    eventCookies.Add(_getEvents, selCookie);
                                    HttpClientHandler eventHandler = new HttpClientHandler
                                    {
                                        CookieContainer = eventCookies,
                                        UseCookies = true,
                                        UseDefaultCredentials = false
                                    };
                                    var datest = start.Split("T")[0].Split("-");
                                    HttpClient eventClient = new HttpClient(eventHandler);
                                    var eventForm = new List<KeyValuePair<string, string>>
                                    {
                                new KeyValuePair<string, string>("showdate", $"{datest[1].TrimStart('0')}/{datest[2]}/{datest[0]}"),
                                new KeyValuePair<string, string>("viewtype", "month"),
                                new KeyValuePair<string, string>("timezone", "-4")
                                    };
                                    var eventContent = new FormUrlEncodedContent(eventForm);
                                    var eventRes = await eventClient.PostAsync(_getEvents, eventContent);
                                    var events = (JObject)JsonConvert.DeserializeObject(await eventRes.Content.ReadAsStringAsync());
                                    var loca = int.Parse(loc.Value);

                                ////start timer
                                //var watch = Stopwatch.StartNew();
                                ////

                                citas.UnionWith(events.SelectToken("events")
                                        .AsParallel()
                                    .Select(e =>
                                    {
                                        var libre = string.IsNullOrWhiteSpace(e[11].ToString());
                                        if (libre)
                                        {
                                            return new Cita
                                            {
                                                Id = int.Parse(e[0].ToString()),
                                                Atencion = e[1].ToString(),
                                                Start = ParseDate(e[2].ToString(), formats),
                                                End = ParseDate(e[3].ToString(), formats),
                                                Lugar = loc.Text,
                                                LocId = int.Parse(loc.Value),
                                                Title = "Libre",
                                                Url = "#",
                                                BackgroundColor = colors[loca],
                                                DoctorId = id
                                            };
                                        }
                                        else
                                        {
                                            var prevision = e[13].ToString();
                                            var run = (int?)int.Parse(e[11].ToString());
                                            var nfi = new NumberFormatInfo { NumberDecimalSeparator = ",", NumberGroupSeparator = "." };
                                            if (run.HasValue)
                                            {
                                                var patient = new Patient { NaturalId = run.Value };
                                                return new Cita
                                                {
                                                    Id = int.Parse(e[0].ToString()),
                                                    Atencion = e[1].ToString(),
                                                    Start = ParseDate(e[2].ToString(), formats),
                                                    End = ParseDate(e[3].ToString(), formats),
                                                    Prevision = prevision,
                                                    Edad = string.IsNullOrWhiteSpace(e[14].ToString()) ?
                                                            null : (int?)int.Parse(Regex.Replace(e[14].ToString(), "\\D", "")),
                                                    Lugar = loc.Text,
                                                    LocId = int.Parse(loc.Value),
                                                    RUN = run.Value,
                                                    Title = patient.Natural.GetRUT(),
                                                    Url = "#",
                                                    BackgroundColor = colors[loca],
                                                    DoctorId = id
                                                };
                                            }
                                            return new Cita
                                            {
                                                Id = int.Parse(e[0].ToString()),
                                                Atencion = e[1].ToString(),
                                                Start = ParseDate(e[2].ToString(), formats),
                                                End = ParseDate(e[3].ToString(), formats),
                                                Prevision = prevision,
                                                Edad = string.IsNullOrWhiteSpace(e[14].ToString()) ?
                                                null : (int?)int.Parse(Regex.Replace(e[14].ToString(), "\\D", "")),
                                                Lugar = loc.Text,
                                                LocId = int.Parse(loc.Value),
                                                Title = $"sin RUT",
                                                Url = "#",
                                                BackgroundColor = colors[loca],
                                                DoctorId = id
                                            };
                                        }
                                    }
                                    ));
                                ////measure time
                                //watch.Stop();
                                //var elapsedMs = watch.ElapsedMilliseconds;
                                //var test = "end";
                                ////end
                            }
                            }
                        );
                        await Task.WhenAll(tasks);
                    }
                }
            }
            return Json(citas);
        }
        public DateTime? ParseDate(string val, string[] formats)
        {
            var success = DateTime.TryParseExact(val,
                    formats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out DateTime start);
            return success ? (DateTime?)start : null;
        }

    }
}