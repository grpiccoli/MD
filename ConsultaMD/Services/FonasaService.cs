﻿using ConsultaMD.Extensions;
using ConsultaMD.Models.VM;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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
            var response = await _nodeServices.InvokeAsync<string>("Services/FonasaService.js", FonasaSettings).ConfigureAwait(false);
            FonasaSettings.BrowserWSEndpoint = response;
            return;
        }
        public async Task CloseBW()
        {
            FonasaSettings.Rut = null;
            await _nodeServices.InvokeAsync<string>("Services/FonasaService.js", FonasaSettings).ConfigureAwait(false);
            return;
        }
        public async Task<Fonasa> GetById(int id)
        {
            FonasaSettings.Rut = RUT.Fonasa(id);
            var response = await _nodeServices.InvokeAsync<string>("Services/FonasaService.js", FonasaSettings).ConfigureAwait(false);
            var fonasaData = JsonConvert.DeserializeObject<Fonasa>(response);
            return fonasaData;
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