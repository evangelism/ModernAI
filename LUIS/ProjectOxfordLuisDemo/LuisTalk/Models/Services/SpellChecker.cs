using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Text.SpellCheck;
using Newtonsoft.Json;
using System.Reflection;

namespace LuisTalk.Models.Services
{
    public class SpellCheckerApi
    {
        private ISpellCheckClient _client;

        public SpellCheckerApi(string subscriptionKey)
        {
            _client = new SpellCheckClient(subscriptionKey);
        }

        public async Task<string> CorrectAsync(string text)
        {
            var response = await _client.GetSuggestionsAsync(text, null, null, CheckMode.Proof);

            // demostration purposes only
            var RawResponse = JsonConvert.SerializeObject(response, Formatting.Indented);

            return SpellCheckClient.ConstructCorrectedTextFromResponse(response, text);
        }
    }
}
