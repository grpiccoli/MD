using ConsultaMD.Models.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ConsultaMD.Services
{
    public class FlowService : IFlow
    {
        public FlowSettings FlowSettings { get; set; }
        public byte[] StringEncode(string text)
        {
            var encoding = new ASCIIEncoding();
            return encoding.GetBytes(text);
        }
        public string GetSignature(string sortedParams)
        {
            using (var hash = new HMACSHA256(StringEncode(FlowSettings.SecretKey)))
            return hash.ComputeHash(StringEncode(sortedParams)).ToString();
        }
        public SortedDictionary<string, string> Sign(SortedDictionary<string, string> ccForm)
        {
            if(ccForm != null)
            {
                var sortedParams = string.Join("", ccForm.Select(p => $"{p.Key}{p.Value}"));
                ccForm.Add("s", GetSignature(sortedParams));
            }
            return ccForm;
        }
        public async Task<Customer> CustomerCreate(string email, int id)
        {
            var uri = new Uri(FlowSettings.EndPoint, "/customer/create");
            var ccForm = new SortedDictionary<string, string>
            {
                { "apiKey", FlowSettings.ApiKey },
                { "name", email },
                { "externalId", id.ToString(CultureInfo.InvariantCulture) }
            };
            using (HttpClient client = new HttpClient())
            using (var formContent = new FormUrlEncodedContent(Sign(ccForm)))
            using (var ccQ = await client.PostAsync(uri, formContent).ConfigureAwait(false))
            {
                if (ccQ.IsSuccessStatusCode)
                {
                    var ccDoc = await ccQ.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var json = JsonConvert.DeserializeObject<Customer>(ccDoc);
                    return json;
                }
                return new Customer();
            }
        }
        public async Task<Register> CustomerRegister(string customerId, Uri returnUrl)
        {
            var uri = new Uri(FlowSettings.EndPoint, "/customer/register");
            var ccForm = new SortedDictionary<string, string>
            {
                { "apiKey", FlowSettings.ApiKey },
                { "customerId", customerId },
                { "url_return", returnUrl?.ToString() }
            };
            using (HttpClient client = new HttpClient())
            using (var formContent = new FormUrlEncodedContent(Sign(ccForm)))
            using (var ccQ = await client.PostAsync(uri, formContent).ConfigureAwait(false))
            {
                if (ccQ.IsSuccessStatusCode)
                {
                    var ccDoc = await ccQ.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var json = JsonConvert.DeserializeObject<Register>(ccDoc);
                    return json;
                }
                return new Register();
            }
        }
        public async Task<Customer> GetRegisterStatus(string token)
        {
            var uri = new Uri(FlowSettings.EndPoint, "/customer/getRegisterStatus");
            var ccForm = new SortedDictionary<string, string>
            {
                { "apiKey", FlowSettings.ApiKey },
                { "token", token }
            };
            using (HttpClient client = new HttpClient())
            using (var formContent = new FormUrlEncodedContent(Sign(ccForm)))
            using (var ccQ = await client.PostAsync(uri, formContent).ConfigureAwait(false))
            {
                if (ccQ.IsSuccessStatusCode)
                {
                    var ccDoc = await ccQ.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var json = JsonConvert.DeserializeObject<Customer>(ccDoc);
                    return json;
                }
                return new Customer();
            }
        }
        public async Task<Customer> CustomerCharge(string token)
        {
            var uri = new Uri(FlowSettings.EndPoint, "/customer/charge");
            var ccForm = new SortedDictionary<string, string>
            {
                { "apiKey", FlowSettings.ApiKey },
                { "customerId", "" },
                { "amount", "" },
                { "subject", "" },
                { "commerceOrder", "" },
                { "currency", "UF" },
                { "optionals", "" }
            };
            using (HttpClient client = new HttpClient())
            using (var formContent = new FormUrlEncodedContent(Sign(ccForm)))
            using (var ccQ = await client.PostAsync(uri, formContent).ConfigureAwait(false))
            {
                if (ccQ.IsSuccessStatusCode)
                {
                    var ccDoc = await ccQ.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var json = JsonConvert.DeserializeObject<Customer>(ccDoc);
                    return json;
                }
                return new Customer();
            }
        }
    }
    public class Register
    {
        public Uri Url { get; set; }
        public string Token { get; set; }
    }
    public class FlowSettings
    {
        public string ApiKey { get; set; }
        public string SecretKey { get; set; }
        public string Currency { get; set; }
        public Uri EndPoint { get; set; }
    }
}
