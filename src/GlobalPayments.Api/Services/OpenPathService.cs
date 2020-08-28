﻿using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.ServiceConfigs.Gateways;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace GlobalPayments.Api.Services
{
    public class OpenPathService
    {
        public static OpenPathResponse PreFlightFraudCheck(AuthorizationBuilder transaction, IOpenPathConfig openPathConfig) {
            var openPathTransaction = new OpenPathTransaction().MapData(transaction);
            openPathTransaction.OpenPathApiKey = openPathConfig.OpenPathApiKey;
            return SendRequest(JsonConvert.SerializeObject(openPathTransaction), openPathConfig.OpenPathApiUrl);
        }

        public static void PostFlightFraudCheck(OpenPathResponse data, Transaction result) {
            // TODO: send request to openpath to update transaction status
        }

        private static OpenPathResponse SendRequest(string jsonContent, string url)
        {
            var result = new OpenPathResponse();
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                using (var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json"))
                {
                    request.Content = stringContent;

                    using (var response = client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).Result)
                    {
                        response.EnsureSuccessStatusCode();
                        var responseJson = response.Content.ReadAsStringAsync().Result;
                        result = JsonConvert.DeserializeObject<OpenPathResponse>(responseJson);
                    }
                }
            }
            return result;
        }
    }
}
