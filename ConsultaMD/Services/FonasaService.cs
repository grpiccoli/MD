using AngleSharp.Html.Parser;
using ConsultaMD.Extensions;
using ConsultaMD.Models.Entities;
using ConsultaMD.Models.VM;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsultaMD.Services
{
    public sealed class FonasaService: IFonasa
    {
        private readonly INodeServices _nodeServices;
        public FonasaSettings FonasaSettings { get; set; }
        public FonasaService(INodeServices nodeServices,
                    IOptions<FonasaSettings> settings)
        {
            FonasaSettings = settings?.Value;
            _nodeServices = nodeServices;
        }
        public async Task Init()
        {
            FonasaSettings.Close = false;
            FonasaSettings.DocData = false;
            var response = await _nodeServices.InvokeAsync<string>("src/scripts/node/mi/FonasaService.js", FonasaSettings).ConfigureAwait(false);
            var fonasaData = JsonConvert.DeserializeObject<Fonasa>(response);
            FonasaSettings.BrowserWSEndpoint = fonasaData.BrowserWSEndpoint;
            return;
        }
        public async Task CloseBW()
        {
            FonasaSettings.Close = true;
            await _nodeServices.InvokeAsync<string>("src/scripts/node/mi/FonasaService.js", FonasaSettings).ConfigureAwait(false);
            return;
        }
        public async Task<Fonasa> GetById(int id)
        {
            FonasaSettings.Close = false;
            FonasaSettings.DocData = false;
            FonasaSettings.Rut = RUT.Fonasa(id);
            var response = await _nodeServices.InvokeAsync<string>("src/scripts/node/mi/FonasaService.js", FonasaSettings).ConfigureAwait(false);
            var fonasaData = JsonConvert.DeserializeObject<Fonasa>(response);
            FonasaSettings.BrowserWSEndpoint = fonasaData.BrowserWSEndpoint;
            return fonasaData;
        }
        public async Task<List<DocFonasa>> GetDocData(int id) 
        {
            FonasaSettings.Close = false;
            FonasaSettings.DocData = true;
            FonasaSettings.Rut = RUT.Format(id,false);
            var response = await _nodeServices.InvokeAsync<string>("src/scripts/node/mi/FonasaService.js", FonasaSettings).ConfigureAwait(false);
            //var places = Regex.Matches(response, @"/\(([^)]+)\)/").Where((m,i) => i % 2 != 0);
            var fonasaData = JsonConvert.DeserializeObject<Fonasa>(response);
            var parser = new HtmlParser();
            var doc = parser.ParseDocument("<html><body><table><tbody>"+fonasaData.Text+"</tbody></table></body></html>");
            var trs = doc.GetElementsByTagName("tr");
            var docs = new List<DocFonasa>();
            foreach (var tr in trs) {
                var tds = tr.GetElementsByTagName("td");
                var button = tds[4].GetElementsByTagName("button").First();
                var docFonasa = new DocFonasa();
                var addressString = Regex.Matches(tds[2].TextContent, @"\(([^)]+)\)");
                if(addressString.Count == 2) docFonasa.Address = addressString[1].Value.Replace("(","", StringComparison.InvariantCulture).Replace(")","", StringComparison.InvariantCulture);
                docFonasa.Commune = button.GetAttribute("data-" + "comlegal");
                docFonasa.Region = button.GetAttribute("data-" + "regionlegal");
                docFonasa.Nivel = int.Parse(button.GetAttribute("data-" + "nivelprestador"), CultureInfo.InvariantCulture);
                docFonasa.PrestacionId = button.GetAttribute("data-" + "codprestacion");
                docFonasa.RutTratante = button.GetAttribute("data-" + "ruttratante");
                docFonasa.NomTratante = button.GetAttribute("data-" + "nomtratante");
                docFonasa.Prestacion = await GetPrestacion(docFonasa.PrestacionId, docFonasa.Nivel).ConfigureAwait(false);
                docs.Add(docFonasa);
            }
            return docs;
        }
        public static async Task<Prestacion> GetPrestacion(string id, int level)
        {
            var loginFormValues = new Dictionary<string, string>
            {
                { "OPTION_SELECTED", "1" },
                { "TXTDESCRIPTION", "" },
                { "TXTPRESTACION", id },
                { "selGrupos", "--SELECCIONE--" },
                { "selSubGrupos", "--SELECCIONE--" }
            };

            CookieContainer getCookies = new CookieContainer();
            var uri = new Uri("https://va.fonasa.cl/sv/valoriza.asp");
            var parser = new HtmlParser();
            var postUri = new Uri(uri, "/sv/valoriza_R.asp");
            using (HttpClientHandler getHandler = new HttpClientHandler
            {
                CookieContainer = getCookies,
                UseCookies = true,
                UseDefaultCredentials = false
            })
            using (HttpClient client = new HttpClient(getHandler))
            using (await client.GetAsync(uri).ConfigureAwait(false))
            using (HttpClientHandler postHandler = new HttpClientHandler { 
                CookieContainer = getCookies,
                UseCookies = true
            })
            using (HttpClient postClient = new HttpClient(postHandler))
            using (FormUrlEncodedContent formContent = new FormUrlEncodedContent(loginFormValues))
            using (HttpResponseMessage postRespose = await postClient.PostAsync(postUri, formContent).ConfigureAwait(false))
            using (var doc = await parser
                .ParseDocumentAsync(await postRespose.Content
                .ReadAsStringAsync().ConfigureAwait(false))
                .ConfigureAwait(false))
            {
                var result = doc.QuerySelectorAll(".MenuConten > tbody > tr > td > table > tbody > tr > td");
                if (result.Any())
                {
                    var prestacion = new Prestacion
                    {
                        Id = Regex.Replace(result[0].TextContent, @"\D", ""),
                        Description = result[1].TextContent
                    };
                    var ammounts = result[level + 1].QuerySelectorAll("td");
                    prestacion.Total = int.Parse(Regex.Replace(ammounts[0].TextContent, @"\D", ""), CultureInfo.InvariantCulture);
                    prestacion.Copago = int.Parse(Regex.Replace(ammounts[1].TextContent, @"\D", ""), CultureInfo.InvariantCulture);
                    return prestacion;
                }
                else
                {
                    return null;
                }
            }
        }
        public async Task<FonasaWebPay> Pay(PaymentData paymentData)
        {
            if(paymentData != null)
            {
                paymentData.AcKey = FonasaSettings.AcKey;
                var response = await _nodeServices.InvokeAsync<string>("src/scripts/node/mi/fonasa.js", paymentData).ConfigureAwait(false);
                var fonasaData = JsonConvert.DeserializeObject<FonasaWebPay>(response);
                return fonasaData;
            }
            return null;
        }
    }
    public class FonasaSettings
    {
        public string AcKey { get; set; }
        public string Rut { get; set; }
        public string BrowserWSEndpoint { get; internal set; }
        public bool DocData { get; set; }
        public bool Close { get; set; }
    }
    public class PaymentData
    {
        public string AcKey { get; set; }
        public string Rut { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string DocRut { get; set; }
        public string Specialty { get; set; }
        public string Region { get; set; }
        public string Commune { get; set; }
        public string PayRut { get; set; }
    }
}
