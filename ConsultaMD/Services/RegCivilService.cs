using ConsultaMD.Extensions;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace ConsultaMD.Services
{
    public class RegCivilService : IRegCivil
    {
        private readonly INodeServices _nodeServices;
        public RegCivilSettings RegCivilSettings { get; set; }
        public RegCivilService(INodeServices nodeServices,
                    IOptions<RegCivilSettings> settings)
        {
            RegCivilSettings = settings?.Value;
            _nodeServices = nodeServices;
        }
        public async Task Init()
        {
            RegCivilSettings.Close = false;
            var response = await _nodeServices.InvokeAsync<string>("src/scripts/node/ps/RegCivil.js", RegCivilSettings).ConfigureAwait(false);
            var RegCivilData = JsonConvert.DeserializeObject<RegCivil>(response);
            RegCivilSettings.BrowserWSEndpoint = RegCivilData.BrowserWSEndpoint;
            RegCivilSettings.Captcha = RegCivilData.Captcha;
            return;
        }
        public async Task CloseBW()
        {
            RegCivilSettings.Close = true;
            await _nodeServices.InvokeAsync<string>("src/scripts/node/ps/RegCivil.js", RegCivilSettings).ConfigureAwait(false);
            return;
        }
        public async Task<bool> IsValid(int rut, int carnet, bool isExt)
        {
            RegCivilSettings.Rut = RUT.Format(rut, false);
            RegCivilSettings.Carnet = carnet;
            RegCivilSettings.Close = false;
            RegCivilSettings.IsExt = isExt;
            var response = await _nodeServices.InvokeAsync<string>("src/scripts/node/ps/RegCivil.js", RegCivilSettings).ConfigureAwait(false);
            var vigente = JsonConvert.DeserializeObject<RegCivil>(response);
            if (vigente.Close)
                await CloseBW().ConfigureAwait(false);
            RegCivilSettings.BrowserWSEndpoint = vigente.BrowserWSEndpoint;
            return vigente.IsValid;
        }
    }
    public class RegCivilSettings
    {
        public string AcKey { get; set; }
        public string Rut { get; set; }
        public int Carnet { get; set; }
        public string BrowserWSEndpoint { get; internal set; }
        public string Captcha { get; set; }
        public bool Close { get; set; }
        public bool IsExt { get; set; }
    }
    public class RegCivil
    {
        public bool IsValid { get; set; }
        public string BrowserWSEndpoint { get; set; }
        public bool Close { get; set; }
        public string Captcha { get; set; }
    }
}
