using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
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
    public class SuperSaludService : ISuperSaludService
    {
//        #region SRCI Registro Civil
//        public async Task<string> SRCI(int rut, int carnet = 0) {
//            var cedula = carnet != 0;
//            var uri = new Uri($"https://portal.sidiv.registrocivil.cl/usuarios-portal/pages/Document{(cedula ? "Request" : "")}Status.xhtml");
//            var parser = new HtmlParser();
//            using HttpClient client = new HttpClient();
//            using var docHome = await parser.ParseDocumentAsync(await client.GetStringAsync(uri)
//.ConfigureAwait(false)).ConfigureAwait(false);
//            var loginFormValues = new Dictionary<string, string>
//                {
//                    { "form", "form" },
//                    { "form:captchaUrl", "initial" },
//                    { "form:run", $"{RUT.Format(rut,false)}" },
//                    { $"form:{(cedula ? "selectDocType" : "styledSelect")}", "CEDULA" },
//                    { "form:inputCaptcha", "" },
//                    { "form:buttonHidden", "" },
//                    { "javax.faces.ViewState", Javax(docHome) }
//                };
//            if (cedula) loginFormValues.Add("form:docNumber", carnet.ToString(CultureInfo.InvariantCulture));
//            using var formContent = new FormUrlEncodedContent(loginFormValues);
//            using HttpResponseMessage qNac = await client.PostAsync(uri, formContent).ConfigureAwait(false);
//            using var docNac = await parser.ParseDocumentAsync(await qNac.Content.ReadAsStringAsync()
//.ConfigureAwait(false)).ConfigureAwait(false);
//            if (cedula)
//            {
//                var estado = GetEstadoCarnet(docNac);
//                if (estado == "Vigente") return "CHILENA";
//                loginFormValues["javax.faces.ViewState"] = Javax(docNac);
//                loginFormValues["form:selectDocType"] = "CEDULA_EXT";
//                using var formContent2 = new FormUrlEncodedContent(loginFormValues);
//                using HttpResponseMessage qExt = await client.PostAsync(uri, formContent2).ConfigureAwait(false);
//                using var docExt = await parser.ParseDocumentAsync(await qExt.Content.ReadAsStringAsync()
//.ConfigureAwait(false)).ConfigureAwait(false);
//                estado = GetEstadoCarnet(docExt);
//                if (estado == "Vigente") return "EXTRANJERA";
//                return null;
//            }
//            else
//            {
//                var tableNac = GetRows(docNac);
//                if (tableNac.Length > 2) return "CHILENA";
//                loginFormValues["javax.faces.ViewState"] = Javax(docNac);
//                loginFormValues["form:styledSelect"] = "CEDULA_EXT";
//                using var formContent3 = new FormUrlEncodedContent(loginFormValues);
//                using var qExt = await client.PostAsync(uri, formContent3).ConfigureAwait(false);
//                using var docExt = await parser.ParseDocumentAsync(await qExt.Content.ReadAsStringAsync()
//.ConfigureAwait(false)).ConfigureAwait(false);
//                var tableExt = GetRows(docExt);
//                if (tableExt.Length > 2) return "EXTRANJERA";
//                return null;
//            }
//        }
//        public static string Javax(IHtmlDocument doc)
//        {
//            return doc?.GetElementsByTagName("input").Last().GetAttribute("value");
//        }
//        public static string GetEstadoCarnet(IHtmlDocument doc)
//        {
//            return doc?.QuerySelector(".setWidthOfSecondColumn").TextContent;
//        }
//        public static IHtmlCollection<IElement> GetRows(IHtmlDocument doc)
//        {
//            return doc?.QuerySelectorAll("tr.rowWidth > td");
//        }
        //#endregion
        #region SIS SuperSalud
        public async Task<SuperData> GetDr(int rut)
        {
            var search = new Uri($"http://webhosting.superdesalud.gob.cl/bases/prestadoresindividuales.nsf/(searchAll2)/Search?SearchView&Query=(FIELD%20rut_pres={rut})");
            var parser = new HtmlParser();
            using (HttpClient hc = new HttpClient())
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
                        using var doc2 = await parser.ParseDocumentAsync(
                            await hc.GetStringAsync(
                                new Uri($"http://webhosting.superdesalud.gob.cl/bases/prestadoresindividuales.nsf/(searchAll2)/{data.Sis}?OpenDocument"))
                            .ConfigureAwait(false)).ConfigureAwait(false);
                        var reporteRows = doc2.QuerySelectorAll(".reporte > table > tbody > tr");
                        //1st row Name
                        data.NameFirst = reporteRows[0].GetElementsByTagName("td")[1].TextContent;
                        //2nd row Title and specialties
                        var antecedentesRow = reporteRows[1].QuerySelectorAll("table.antecedente > tbody > tr");
                        data.TitleId = antecedentesRow[0].QuerySelector("span").GetAttribute("ref");
                        using IHtmlDocument doc3 = await parser.ParseDocumentAsync(
                            await hc.GetStringAsync(
                                new Uri($"http://webhosting.superdesalud.gob.cl/bases/prestadoresindividuales.nsf/(AntecRegxRut2)//{data.TitleId}?open"))
                            .ConfigureAwait(false)).ConfigureAwait(false);
                        var cells3 = doc3.GetElementsByTagName("td");
                        var date = Regex.Replace(Regex.Replace(cells3[1].TextContent, @"^\D+", ""), @"de ", "").Trim();
                        data.TitleDate = DateTime.ParseExact(date, "d MMMM yyyy", new CultureInfo("es-CL"));
                        //especialidades
                        //foreach (var esp in antecedentesRow.Skip(1))
                        //{

                        //}

                        //3rd row Birth RUT Sex
                        var tds = reporteRows[2].GetElementsByTagName("td");
                        data.Birth = DateTime.ParseExact(tds[1].TextContent, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                        data.Sex = tds[5].TextContent == "Masculino";
                        //4th row Nationality Region
                        tds = reporteRows[3].GetElementsByTagName("td");
                        data.Nationality = tds[1].TextContent;
                        //5th row Reg
                        tds = reporteRows[4].GetElementsByTagName("td");
                        data.Id = int.Parse(tds[1].TextContent.Trim(), CultureInfo.InvariantCulture);
                        data.Registry = DateTime.ParseExact(tds[3].TextContent, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                        return data;
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
