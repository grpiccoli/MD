﻿using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConsultaMD.Services
{
    public class TwilioVerifyClient
    {
        private readonly HttpClient _client;
        public TwilioVerifyClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<TwilioSendVerificationCodeResponse> StartVerification(int countryCode, string phoneNumber)
        {
            var requestContent = new FormUrlEncodedContent(new[] {
               new KeyValuePair<string, string>("via", "sms"),
               new KeyValuePair<string, string>("country_code", countryCode.ToString(CultureInfo.InvariantCulture)),
               new KeyValuePair<string, string>("phone_number", phoneNumber),
           });

            var response = await _client.PostAsync(new Uri("protected/json/phones/verification/start"), requestContent).ConfigureAwait(false);

            requestContent.Dispose();

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            // this will throw if the response is not valid
            return JsonConvert.DeserializeObject<TwilioSendVerificationCodeResponse>(content);
        }

        public async Task<TwilioCheckCodeResponse> CheckVerificationCode(int countryCode, string phoneNumber, string verificationCode)
        {
            var queryParams = new Dictionary<string, string>()
           {
               {"country_code", countryCode.ToString(CultureInfo.InvariantCulture)},
               {"phone_number", phoneNumber},
               {"verification_code", verificationCode },
           };

            var url = new Uri(QueryHelpers.AddQueryString("protected/json/phones/verification/check", queryParams));

            var response = await _client.GetAsync(url).ConfigureAwait(false);

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            // this will throw if the response is not valid
            return JsonConvert.DeserializeObject<TwilioCheckCodeResponse>(content);
        }

    }
    public class TwilioCheckCodeResponse
    {
        public string Message { get; set; }
        public bool Success { get; set; }
    }

    public class TwilioSendVerificationCodeResponse
    {
        public string Carrier { get; set; }
        public bool IsCellphone { get; set; }
        public string Message { get; set; }
        public string SecondsToExpire { get; set; }
        public Guid Uuid { get; set; }
        public bool Success { get; set; }
    }

}
