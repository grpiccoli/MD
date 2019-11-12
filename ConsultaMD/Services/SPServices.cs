using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using ConsultaMD.Data;
using ConsultaMD.Extensions;
using ConsultaMD.Models.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsultaMD.Services
{
    public static class SPServices
    {
        #region SRCI Registro Civil
        public static async Task<string> SRCI(int rut, int carnet = 0) {
            var cedula = carnet != 0;
            var uri = new Uri($"https://portal.sidiv.registrocivil.cl/usuarios-portal/pages/Document{(cedula ? "Request" : "")}Status.xhtml");
            var parser = new HtmlParser();
            using (HttpClient client = new HttpClient())
            {
                using (var docHome = await parser.ParseDocumentAsync(await client.GetStringAsync(uri)
                    .ConfigureAwait(false)).ConfigureAwait(false))
                {
                    var loginFormValues = new Dictionary<string, string>
                    {
                        { "form", "form" },
                        { "form:run", $"{RUT.Format(rut,false)}" },
                        { $"form:{(cedula ? "selectDocType" : "styledSelect")}", "CEDULA" },
                        { "form:buttonHidden", "" },
                        { "javax.faces.ViewState", Javax(docHome) }
                    };
                    if (cedula) loginFormValues.Add("form:docNumber", carnet.ToString(CultureInfo.InvariantCulture));
                    var formContent = new FormUrlEncodedContent(loginFormValues);
                    using (var qNac = await client.PostAsync(uri, formContent).ConfigureAwait(false))
                    {
                        formContent.Dispose();
                        using (var docNac = await parser.ParseDocumentAsync(await qNac.Content.ReadAsStringAsync()
                            .ConfigureAwait(false)).ConfigureAwait(false))
                        {
                            if (cedula) {
                                var estado = GetEstadoCarnet(docNac);
                                if (estado == "Vigente") return "CHILENA";
                                loginFormValues["javax.faces.ViewState"] = Javax(docNac);
                                loginFormValues["form:selectDocType"] = "CEDULA_EXT";
                                formContent = new FormUrlEncodedContent(loginFormValues);
                                using (var qExt = await client.PostAsync(uri, formContent).ConfigureAwait(false))
                                {
                                    formContent.Dispose();
                                    using (var docExt = await parser.ParseDocumentAsync(await qExt.Content.ReadAsStringAsync()
                                        .ConfigureAwait(false)).ConfigureAwait(false))
                                    {
                                        estado = GetEstadoCarnet(docExt);
                                        if (estado == "Vigente") return "EXTRANJERA";
                                        return null;
                                    }
                                }
                            }
                            else
                            {
                                var tableNac = GetRows(docNac);
                                if (tableNac.Length > 2) return "CHILENA";
                                loginFormValues["javax.faces.ViewState"] = Javax(docNac);
                                loginFormValues["form:styledSelect"] = "CEDULA_EXT";
                                formContent = new FormUrlEncodedContent(loginFormValues);
                                using (var qExt = await client.PostAsync(uri, formContent).ConfigureAwait(false))
                                {
                                    formContent.Dispose();
                                    using (var docExt = await parser.ParseDocumentAsync(await qExt.Content.ReadAsStringAsync()
                                        .ConfigureAwait(false)).ConfigureAwait(false))
                                    {
                                        var tableExt = GetRows(docExt);
                                        if (tableExt.Length > 2) return "EXTRANJERA";
                                        return null;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public static string Javax(IHtmlDocument doc)
        {
            return doc?.GetElementsByTagName("input").Last().GetAttribute("value");
        }
        public static string GetEstadoCarnet(IHtmlDocument doc)
        {
            return doc?.QuerySelector(".setWidthOfSecondColumn").TextContent;
        }
        public static IHtmlCollection<IElement> GetRows(IHtmlDocument doc)
        {
            return doc?.QuerySelectorAll("tr.rowWidth > td");
        }
        #endregion
        #region SIS SuperSalud
        public static async Task<SuperData> GetDr(int rut)
        {
            var search = new Uri($"http://webhosting.superdesalud.gob.cl/bases/prestadoresindividuales.nsf/(searchAll2)/Search?SearchView&Query=(FIELD%20rut_pres={rut})");
            var parser = new HtmlParser();
            using (HttpClient hc = new HttpClient()) {
                using (var doc = await parser.ParseDocumentAsync(await hc.GetStringAsync(search).ConfigureAwait(false)).ConfigureAwait(false)) {
                    var count = doc.GetElementsByTagName("maxview").First().TextContent.Trim();
                    if (count == "1") {
                        var cells = doc.GetElementsByTagName("td");
                        var tmp = cells[0];
                        var details = tmp.GetElementsByTagName("a").OfType<IHtmlAnchorElement>().First().Href;
                        var data = new SuperData
                        {
                            LastFirst = tmp.TextContent,
                            Title = cells[2].TextContent,
                            Institution = cells[3].TextContent,
                            Specialties = cells[4].TextContent.Split("<br>"),
                            Sis = details.Split("?")[0].Split("/").Last()
                        };
                        if (data.Dr) {
                            using (var doc2 = await parser.ParseDocumentAsync(
                                await hc.GetStringAsync(
                                    new Uri($"http://webhosting.superdesalud.gob.cl/bases/prestadoresindividuales.nsf/(searchAll2)/{data.Sis}?OpenDocument"))
                                .ConfigureAwait(false)).ConfigureAwait(false))
                            {
                                var cells2 = doc2.GetElementsByTagName("td");
                                data.NameFirst = cells2[8].TextContent;
                                data.Birth = DateTime.ParseExact(cells2[17].TextContent, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                                data.Nationality = cells2[23].TextContent;
                                data.Id = int.Parse(cells2[27].TextContent.Trim(), CultureInfo.InvariantCulture);
                                data.Sex = cells2[21].TextContent == "Masculino";
                                data.Registry = DateTime.ParseExact(cells2[29].TextContent, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                                data.TitleId = cells2[11].FirstElementChild.GetAttribute("ref");
                                using (var doc3 = await parser.ParseDocumentAsync(
                                    await hc.GetStringAsync(
                                        new Uri($"http://webhosting.superdesalud.gob.cl/bases/prestadoresindividuales.nsf/(AntecRegxRut2)//{data.TitleId}?open"))
                                    .ConfigureAwait(false)).ConfigureAwait(false))
                                {
                                    var cells3 = doc3.GetElementsByTagName("td");
                                    var date = Regex.Replace(Regex.Replace(cells3[1].TextContent, @"^\D+", ""), @"de ", "").Trim();
                                    data.TitleDate = DateTime.ParseExact(date, "d MMMM yyyy", new CultureInfo("es-CL"));
                                    return data;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }
        #endregion
    }
    public class SuperData
    {
        public string LastFirst { get; set; }
        public string Sis { get; set; }
        public string Title { get; set; }
        public DateTime TitleDate { get; set; }
        public string TitleId { get; set; }
        public bool Dr 
        {
            get 
            {
                return Title == "Médico Cirujano";
            }
            private set 
            {
                Dr = value;
            } 
        }
        public string Institution { get; set; }
        public IEnumerable<string> Specialties { get; set; }
        public IEnumerable<DoctorSpecialty> GetSpecialties { get; set; }
        public string NameFirst { get; set; }
        public DateTime Birth { get; set; }
        public string Nationality { get; set; }
        public int Id { get; set; }
        public bool Sex { get; set; }
        public DateTime Registry { get; set; }
    }
}
