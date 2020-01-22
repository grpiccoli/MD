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
        //public async Task<bool> Init()
        //{
        //    var RegCivilData = await VAsync().ConfigureAwait(false);
        //    while (!RegCivilData.IsValid)
        //    {
        //        await CloseBW().ConfigureAwait(false);
        //        RegCivilData = await VAsync().ConfigureAwait(false);
        //    }
        //    RegCivilSettings.BrowserWSEndpoint = RegCivilData.BrowserWSEndpoint;
        //    RegCivilSettings.Captcha = RegCivilData.Captcha;
        //    return true;
        //}
        public async Task<RegCivil> VAsync()
        {
            RegCivilSettings.Close = false;
            var eval = true;
            var response = string.Empty;
            while (eval)
            {
                response = await _nodeServices
                    .InvokeAsync<string>("src/scripts/node/ps/RegCivil.js", RegCivilSettings)
                    .ConfigureAwait(false);
                eval = response.Contains("Error", System.StringComparison.InvariantCulture);
            }
            var data = JsonConvert.DeserializeObject<RegCivil>(response);
            return data;
        }
        //public async Task CloseBW()
        //{
        //    RegCivilSettings.Close = true;
        //    await _nodeServices.InvokeAsync<string>("src/scripts/node/ps/RegCivil.js", RegCivilSettings).ConfigureAwait(false);
        //    return;
        //}
        public async Task<bool> Test()
        {
            var data = await VAsync().ConfigureAwait(false);
            return data.IsValid;
        }
        public async Task<bool> IsValid(int rut, int carnet, bool isExt)
        {
            RegCivilSettings.Rut = RUT.Format(rut, false);
            RegCivilSettings.Carnet = carnet;
            RegCivilSettings.IsExt = isExt;
            return await Test().ConfigureAwait(false);
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
