using ConsultaMD.Extensions;
using Microsoft.Extensions.Options;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace ConsultaMD.Services
{
    public class RegCivilService : IRegCivil
    {
        private readonly IPuppet _puppet;
        private readonly RegCivilSettings _settings;
        public RegCivilService(
            IPuppet puppet,
            IOptions<RegCivilSettings> settings)
        {
            _puppet = puppet;
            _settings = settings?.Value;
        }
//        public async Task Init()
//        {
//            var eval = true;
//            while (eval)
//            {
//                var open = true;
//                Page page = null;
//                while (open)
//                {
//                    try
//                    {
//                        page = await _puppet
//                        .GetPageAsync(_settings.Url, _settings.Block)
//                        .ConfigureAwait(false);
//                        open = false;
//                    }
//                    catch (TimeoutException ex)
//                    {
//                        Console.WriteLine(ex);
//                        await page.Browser.CloseAsync().ConfigureAwait(false);
//                    }
//                };
//                var captcha = await _puppet
//                    .GetCaptchaAsync(page, "[id$='captchaPanel'] > img", "RegCivil")
//                    .ConfigureAwait(false);
//                var body = GetBody(_settings.InitRut, _settings.InitCarnet, captcha);
//                var response = await page.EvaluateFunctionAsync<string>(
//@$"async () => await fetch('{_settings.Url.AbsoluteUri}', {{
//    method: 'POST',
//    headers: new Headers({{ 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' }}),
//    body: `{body}&javax.faces.ViewState=${{document.querySelector(""[id$='ViewState']"").value}}`
//}}).then(response => response.text())")
//        .ConfigureAwait(false);
//                eval = response.Contains("Sesión no válida", StringComparison.InvariantCulture)
//                        || response.Contains("Error", StringComparison.InvariantCulture)
//                        || response.Contains("Por favor, Intente nuevamente", StringComparison.InvariantCulture)
//                        || response.Contains("La información ingresada no corresponde en nuestros registros",
//                        StringComparison.InvariantCulture);
//                if (!eval)
//                {
//                    _settings.WSE = page.Browser.WebSocketEndpoint;
//                    _settings.Captcha = captcha;
//                }
//                else
//                {
//                    await page.Browser.CloseAsync().ConfigureAwait(false);
//                }
//            }
//        }
        public async Task<bool> IsValidAsync(int rut, int carnet, bool isExt)
        {
            bool eval = true, valid = false;
            while (eval)
            {
                var open = true;
                Page page = null;
                while (open)
                {
                    try
                    {
                        page = await _puppet
                        .GetPageAsync(_settings.Url, _settings.Block)
                        .ConfigureAwait(false);
                        open = false;
                    }
                    catch (TimeoutException ex)
                    {
                        Console.WriteLine(ex);
                        await page.Browser.CloseAsync().ConfigureAwait(false);
                    }
                };
                var captcha = await _puppet
                    .GetCaptchaAsync(page, "[id$='captchaPanel'] > img", "RegCivil")
                    .ConfigureAwait(false);
                var body = GetBody(rut, carnet, captcha, isExt);
                var response = await page.EvaluateFunctionAsync<string>(
@$"async () => await fetch('{_settings.Url.AbsoluteUri}', {{
    method: 'POST',
    headers: new Headers({{ 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' }}),
    body: `{body}&javax.faces.ViewState=${{document.querySelector(""[id$='ViewState']"").value}}`
}}).then(r => r.text())")
                    .ConfigureAwait(false);
                var notValid = response.Contains("La información ingresada no corresponde en nuestros registros",
                        StringComparison.InvariantCulture);
                valid = response.Contains("Vigente", StringComparison.InvariantCulture);
                //eval = (response.Contains("Sesión no válida", StringComparison.InvariantCulture)
                //    || response.Contains("Error", StringComparison.InvariantCulture))
                //    || response.Contains("Por favor, Intente nuevamente", StringComparison.InvariantCulture)
                //    ||  || (response.Contains("La información ingresada no corresponde en nuestros registros",
                //        StringComparison.InvariantCulture))
                eval = notValid == valid;
                await page.Browser.CloseAsync().ConfigureAwait(false);
            }
            return valid;
        }
        public static string GetBody(int rut, int carnet, string captcha, bool ext = false)
        {
            var suffix = ext ? "_EXT" : "";
            var parameters = new Dictionary<string, string>
                {
                    { "form", "form" },
                    { "form:captchaUrl", "initial" },
                    { "form:run", RUT.Format(rut,false) },
                    { "form:selectDocType", $"CEDULA{suffix}" },
                    { "form:docNumber", carnet.ToString(CultureInfo.InvariantCulture) },
                    { "form:inputCaptcha", captcha },
                    { "form:buttonHidden", "" }
                };
            return string.Join("&", parameters.Select(p =>
                    string.Format(CultureInfo.InvariantCulture,
                    "{0}={1}", p.Key.Replace(":", "%3A",
                    StringComparison.InvariantCultureIgnoreCase), p.Value)));
        }
    }
    public class RegCivilSettings
    {
        public Uri Url { get; set; }
        public int InitRut { get; set; }
        public int InitCarnet { get; set; }
        public string Captcha { get; set; }
        public string WSE { get; set; }
        public List<string> Block { get; } = new List<string>{
                    "google",
                    "css",
                    "img",
                    "assets"
                };
    }
}
