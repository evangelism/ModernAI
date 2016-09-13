using LuisTalk.Utilities;
using System;
using System.Collections.Generic;
//using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Newtonsoft.Json;

namespace LuisTalk.Models.Services
{
    public class SentimentAnalysisService
    {
        private string _subscriptionKey;
        private string _baseUri = @"https://api.datamarket.azure.com/data.ashx/amla/text-analytics/v1/GetSentiment";

        public object JsonConver { get; private set; }

        public SentimentAnalysisService(string subscriptionKey)
        {
            Check.Required<ArgumentNullException>(() => subscriptionKey != null);

            _subscriptionKey = subscriptionKey;
        }

        public async Task<float> GetSentiment(string text)
        {
            Check.Required<ArgumentNullException>(() => text != null);

            using (var client = new HttpClient())
            {
                try
                {
                    var response = await _baseUri.SetQueryParam("Text", text)
                        .WithHeader("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes("AccountKey:" + _subscriptionKey)))
                        .WithHeader("Accept", "application/json")
                        .GetStringAsync();

                    return JsonConvert.DeserializeObject<SentimentResult>(response).Score;
                }
                catch (Exception e)
                {
                    throw;
                }
            }
        }
    }

    public class SentimentResult
    {
        public float Score { get; set; }
    }
}
