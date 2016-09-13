using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Flurl;
using Newtonsoft.Json;
using LuisTalk.Utilities;

namespace LuisTalk.Models.Services
{
    public class LuisService
    {
        private string _baseAddress = @"https://api.projectoxford.ai/luis/v1/application";
        private string _id;
        private string _subscriptionKey;

        public LuisService(string id, string subscriptionKey)
        {
            _id = id;
            _subscriptionKey = subscriptionKey;
        }

        public async Task<LuisResult> GetIntent(string text)
        {
            using (var client = new HttpClient())
            {
                var url = _baseAddress.SetQueryParam("id", _id)
                    .SetQueryParam("subscription-key", _subscriptionKey)
                    .SetQueryParam("q", text);

                var response = await client.GetStringAsync(url);
                var luisResponse = JsonConvert.DeserializeObject<LuisResponse>(response);
                return LuisResult.FromLuisResponse(luisResponse);
            }
        }
    }

    public class LuisIntent
    {
        public string Intent { get; set; }
        public double Score { get; set; }
    }

    public class LuisEntity
    {
        public string Entity { get; set; }
        public string Type { get; set; }
    }

    public class LuisResponse
    {
        public string Query { get; set; }
        public LuisIntent[] Intents { get; set; }
        public LuisEntity[] Entities { get; set; }
    }

    public class LuisResult
    {
        public string Intent { get; private set; }
        public Dictionary<string, string> Entities { get; private set; }

        public LuisResult(string intent, Dictionary<string, string> entities)
        {
            Check.Required<ArgumentNullException>(() => intent != null);
            Check.Required<ArgumentNullException>(() => entities != null);

            Intent = intent;
            Entities = entities;
        }

        public static LuisResult FromLuisResponse(LuisResponse response)
        {
            Check.Required<ArgumentNullException>(() => response != null);

            var intent = response.Intents?[0]?.Intent;
            var entities = response.Entities?.ToDictionary(e => e.Type, e => e.Entity) ?? new Dictionary<string, string>();

            return new LuisResult(intent, entities);
        }
    }
}
