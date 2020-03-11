using AngleSharp.Html.Parser;
using ConsultaMD.Extensions;
using ConsultaMD.Models.Entities;
using ConsultaMD.Models.VM;
using PuppeteerSharp;
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
        private readonly IPuppet _puppet;
        private readonly Uri _bonoWeb;
        private static readonly Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public FonasaService(IPuppet puppet)
        {
            _bonoWeb = new Uri("https://bonowebfon.fonasa.cl/");
            _puppet = puppet;
        }
        public static async Task<T> BonoAsync<T>(Page page, string ajaxFn, string body, bool json = true)
        {
            if (page == null) throw new EvaluationFailedException();
            var r = json ? "json" : "text";
            return await page.EvaluateFunctionAsync<T>(
@$"async () => await fetch(urlAjax('bono', '{ajaxFn}'), {{
    method: 'POST',
    headers: new Headers({{ 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' }}),
    body: '{body}'
}}).then(response => response.{r}())")
    .ConfigureAwait(false);
        }
        public async Task<Fonasa> GetByIdAsync(int id, bool doc)
        {
            var eval = true;
            var fonasa = new Fonasa();
            var block = new List<string>
            {
                    "vendor",
                    "css",
                    "google",
                    "image"
            };
            while (eval)
            {
                var page = await _puppet
                .GetPageAsync(_bonoWeb, block)
                .ConfigureAwait(false);
                fonasa = await BonoAsync<Fonasa>(page,
                    "execWSCertifPagador",
                    $"RutPagador={RUT.Fonasa(id)}")
                    .ConfigureAwait(false);
                eval = fonasa == null;
                if (doc)
                {
                    var response = await BonoAsync<string>(page,
                        "BuscaPorProfesional",
                        $"TipoBusquedaProfesional=RutProfesional&PalabraClave={RUT.Format(id, false)}&Especialidad=0&Region=0&Comuna=0", json: false)
                        .ConfigureAwait(false);
                    var hasFonasaWeb = response.Contains("ERROR", StringComparison.InvariantCultureIgnoreCase);
                    if(hasFonasaWeb)
                        fonasa.Docs.AddRange(await GetDocDataAsync(response).ConfigureAwait(false));
                }
                await page.Browser.CloseAsync().ConfigureAwait(false);
            }
            return fonasa;
        }
        public static async Task<List<DocFonasa>> GetDocDataAsync(string response)
        {
            var parser = new HtmlParser();
            var doc = await parser.ParseDocumentAsync("<html><body><table><tbody>" + response + "</tbody></table></body></html>").ConfigureAwait(false);
            var espLocations = doc.GetElementsByTagName("tr");
            var docs = new List<DocFonasa>();
            foreach (var espLocation in espLocations)
            {
                var fields = espLocation.GetElementsByTagName("td");
                var button = fields[4].GetElementsByTagName("button").First();
                var docFonasa = new DocFonasa();
                var addressString = Regex.Matches(fields[2].TextContent, @"\(([^)]+)\)");
                if (addressString.Count == 2) docFonasa.Address = addressString[1].Value.Replace("(", "", StringComparison.InvariantCulture).Replace(")", "", StringComparison.InvariantCulture);
                docFonasa.Commune = button.GetAttribute("data-" + "comlegal");
                docFonasa.Region = button.GetAttribute("data-" + "regionlegal");
                docFonasa.NivelPrestador = int.Parse(button.GetAttribute("data-" + "nivelprestador"), CultureInfo.InvariantCulture);
                docFonasa.NomPrestador = button.GetAttribute("data-" + "nomprestador");
                docFonasa.RutPrestador = button.GetAttribute("data-" + "rutprestador");
                docFonasa.CodEspeciali = int.Parse(button.GetAttribute("data-" + "codespeciali"), CultureInfo.InvariantCulture);
                docFonasa.PrestacionId = button.GetAttribute("data-" + "codprestacion");
                docFonasa.RutTratante = button.GetAttribute("data-" + "ruttratante");
                docFonasa.NomTratante = button.GetAttribute("data-" + "nomtratante");
                docFonasa.Prestacion = await GetPrestacionAsync(docFonasa.PrestacionId, docFonasa.NivelPrestador).ConfigureAwait(false);
                docs.Add(docFonasa);
            }
            return docs;
        }
        public static async Task<Prestacion> GetPrestacionAsync(string id, int level)
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
                        Description = result[1].TextContent.Trim()
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
        public async Task<WebPayResponse> PayAsync(PaymentData paymentData)
        {
            if(paymentData != null)
            {
                var eval = true;
                var webpay = new WebPayResponse();
                var block = new List<string>
                {
                    "vendor",
                    "css",
                    "google",
                    "image"
                };
                while (eval)
                {
                    var open = true;
                    Page page = null;
                    while (open)
                    {
                        try
                        {
                            page = await _puppet
                            .GetPageAsync(_bonoWeb)
                            .ConfigureAwait(false);
                            open = false;
                        }
                        catch (TimeoutException ex)
                        {
                            Console.WriteLine(ex);
                            await page.Browser.CloseAsync().ConfigureAwait(false);
                        }
                    };

                    var form = "RutBeneficiario=" + paymentData.RutBeneficiario
                        + "&captcha_code=" + RandomString(6)
                        + "&CelularNotificacion=" + paymentData.CelularNotification
                        + "&EmailNotificacion=" + paymentData.EmailNotification
                        + "&DataBenef={\"estado\":0,\"extApellidoPat\":\"" + paymentData.DataBenef.ExtApellidoPat
                        + "\",\"extApellidoMat\":\"" + paymentData.DataBenef.ExtApellidoMat
                        + "\",\"extNombres\":\"" + paymentData.DataBenef.ExtNombres
                        + "\",\"extSexo\":\"" + paymentData.DataBenef.ExtSexo
                        + "\",\"extFechaNacimi\":\"" + paymentData.DataBenef.ExtFechaNacimi
                        + "\",\"extNomCotizante\":\"" + paymentData.DataBenef.ExtNomCotizante
                        + "\",\"extGrupoIng\":\"" + paymentData.DataBenef.ExtGrupoIng
                        + "\",\"FechaNacimi\":\"" + paymentData.DataBenef.FechaNacimi
                        + "\",\"EdadBeneficiario\":" + paymentData.DataBenef.EdadBeneficiario
                        + "}&RutPrestador=" + paymentData.RutPrestador
                        + "&NomPrestador=" + paymentData.NomPrestador
                        + "&NivelPrestador=" + paymentData.NivelPrestador.ToString(CultureInfo.InvariantCulture)
                        + "&CodEspeciali=" + paymentData.CodEspeciali.ToString(CultureInfo.InvariantCulture)
                        + "&CodPrestacion=" + paymentData.PrestacionId
                        + "&ComLegal=" + paymentData.Commune
                        + "&RegionLegal=" + paymentData.Region
                        + "&RutTratante=" + paymentData.RutTratante
                        + "&NomTratante=" + paymentData.NomTratante
                        + "&a=&b=&c=";

                    var cat = await BonoAsync<ValidaCAT>(page, "validaCAT", form).ConfigureAwait(false);

                    var payForm = form.Replace("&a=&b=&c=", $"&a={cat.Datos.A}&b={cat.Datos.B}&c={cat.Datos.C}", StringComparison.InvariantCultureIgnoreCase);

                    var valores = await page.EvaluateFunctionAsync<string[]>($@"async () => 
await fetch('https://bonowebfon.fonasa.cl/index.php?controller=bono&action=pago&v=18-12-2015_1.47', {{
    method: 'POST',
    headers: new Headers({{ 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' }}),
    body: '{payForm}'
}}).then(r => r.text()).then(data => {{
    var w = window.open('about:blank');
	w.document.open();
	w.document.write(data);
	w.document.close();
    let regexp = /\$.*\.-/g;
    return data.match(regexp);
}})").ConfigureAwait(false);
                    var json = await BonoAsync<Fonasa>(page, "execWSCertifPagador", $"RutPagador={paymentData.PayRut}").ConfigureAwait(false);
                    var newTarget = await page.Browser.WaitForTargetAsync(target => target.Url == "about:blank").ConfigureAwait(false);
                    var newPage = await newTarget.PageAsync().ConfigureAwait(false);
                    //var copagoEl = await newPage.QuerySelectorAsync("#ValorCopago").ConfigureAwait(false);
                    //var copagoHandle = await copagoEl.GetPropertyAsync("value").ConfigureAwait(false);
                    //var copago = await copagoHandle.JsonValueAsync().ConfigureAwait(false);
                    webpay.Total = int.Parse(Regex.Replace(valores[0], @"\D", ""), CultureInfo.InvariantCulture);
                    webpay.Copago = int.Parse(Regex.Replace(valores[2], @"\D", ""), CultureInfo.InvariantCulture);
                    webpay = await BonoAsync<WebPayResponse>(newPage, 
                        "pagar", 
                        "RutPagador=" + paymentData.PayRut 
                        + "&CelularPagador=" + paymentData.CelularNotification
                        + "&MailPagador=" + paymentData.EmailNotification
                        + "&ValorCopago=" + webpay.Copago
                        + "&b=" + cat.Datos.B
                        + "&CelularNotificacion=" + paymentData.CelularNotification
                        + "&EmailNotificacion=" + paymentData.EmailNotification
                        + "&NombrePagador=" + json.Nombre).ConfigureAwait(false);

                    if (webpay.Codigo == 0)
                    {
                        await page.Browser.CloseAsync().ConfigureAwait(false);
                    }
                    else
                    {
                        await page.Browser.CloseAsync().ConfigureAwait(false);
                    }
                    eval = false;
                }
                return webpay;
            }
            return null;
        }
        public static async Task SubmitCaptchaAsync(Page page, string rut, string captcha)
        {
            if (page == null) return;
            await page.TypeAsync("#RutBeneficiario", rut).ConfigureAwait(false);
            await page.TypeAsync("#captcha_code", captcha).ConfigureAwait(false);
            await page.ClickAsync("#btnCertifBenef").ConfigureAwait(false);
        }
    }
    public class WebPayResponse
    {
        public int Codigo { get; set; }
        public string Mensaje { get; set; }
        public Datos Datos { get; set; }
        public int Total { get; set; }
        public int Copago { get; set; }
    }
    public class Datos
    {
        public string Token { get; set; }
        public Uri Uri { get; set; }
    }
}
