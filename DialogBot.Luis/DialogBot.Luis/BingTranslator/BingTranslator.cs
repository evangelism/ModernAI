using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace MSEvangelism.BingTranslator
{
    public class BingTranslatorClient
    {
        private string _clientId;
        private string _secret;
        private string _token = null;
        private DateTime _tokenRenew = DateTime.MinValue;

        public BingTranslatorClient(string clientId, string secret)
        {
            _clientId = clientId;
            _secret = secret;
        }

        public async Task<string> Translate(string text, string fromLocale, string toLocale)
        {
            if (fromLocale == null) throw new ArgumentNullException(nameof(fromLocale));
            if (toLocale == null) throw new ArgumentNullException(nameof(toLocale));
            if (String.IsNullOrWhiteSpace(text)) return text;
            if (toLocale == fromLocale) return text;
            var at = await GetAccessToken();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + at);
                string uri = Uri.EscapeUriString($"http://api.microsofttranslator.com/v2/Http.svc/Translate?text={text}&to={toLocale}&from={fromLocale}");
                string response = await client.GetStringAsync(uri);
                return ExtractTranslation(response);
            }
        }

        private async Task<string> GetAccessToken()
        {
            if (_tokenRenew.AddMinutes(9) > DateTime.Now && _token != null) return _token;
            var url = @"https://api.cognitive.microsoft.com/sts/v1.0/issueToken";
            string res = null;
            using (var http = new HttpClient())
            {
                http.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _secret);
                var resp = await http.PostAsync(url,new StringContent(""));
                res = await resp.Content.ReadAsStringAsync();
            }
            _token = res;
            _tokenRenew = DateTime.Now;
            return res;
        }

        private string ExtractTranslation(string response)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));

            var xTranslation = new System.Xml.XmlDocument();
            xTranslation.LoadXml(response);
            return xTranslation.InnerText;
        }
    }
}