using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace WeatherBotDemo.BingTranslator
{
    public class BingTranslatorClient
    {
        private string _clientId;
        private string _secret;

        public BingTranslatorClient(string clientId, string secret)
        {
            if (clientId == null) throw new ArgumentNullException(nameof(clientId));
            if (secret == null) throw new ArgumentNullException(nameof(secret));

            _clientId = clientId;
            _secret = secret;
        }

        public async Task<string> Translate(string text, string fromLocale, string toLocale)
        {
            if (fromLocale == null) throw new ArgumentNullException(nameof(fromLocale));
            if (toLocale == null) throw new ArgumentNullException(nameof(toLocale));
            if (String.IsNullOrWhiteSpace(text)) return text;
            if (toLocale == fromLocale) return text;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + GetAccessToken());

                try
                {
                    string uri = Uri.EscapeUriString($"http://api.microsofttranslator.com/v2/Http.svc/Translate?text={text}&to={toLocale}&from={fromLocale}");
                    string response = await client.GetStringAsync(uri);
                    return ExtractTranslation(response);
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        #region Helper methods from https://github.com/MicrosoftTranslator/CSharp-WPF-Example/blob/master/MSTranslatorTAPDemo/MainWindow.xaml.cs
        private string GetAccessToken()
        {
            String strTranslatorAccessURI = "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13";
            String strRequestDetails = string.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&scope=http://api.microsofttranslator.com", HttpUtility.UrlEncode(_clientId), HttpUtility.UrlEncode(_secret));

            System.Net.WebRequest webRequest = System.Net.WebRequest.Create(strTranslatorAccessURI);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "POST";

            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(strRequestDetails);
            webRequest.ContentLength = bytes.Length;
            using (System.IO.Stream outputStream = webRequest.GetRequestStream())
            {
                outputStream.Write(bytes, 0, bytes.Length);
            }
            System.Net.WebResponse webResponse = webRequest.GetResponse();

            System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(AdmAccessToken));
            
            AdmAccessToken token = (AdmAccessToken)serializer.ReadObject(webResponse.GetResponseStream());

            return token.access_token;
        }

        private string ExtractTranslation(string response)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));

            var xTranslation = new System.Xml.XmlDocument();
            xTranslation.LoadXml(response);
            return xTranslation.InnerText;
        }
        #endregion
    }
}