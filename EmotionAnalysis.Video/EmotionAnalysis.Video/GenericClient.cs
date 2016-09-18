using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Evangelism.ProjectOxford
{
    public class GenericClient
    {
        public string ApiKey { get; set; }

        public GenericClient(string ApiKey)
        {
            this.ApiKey = ApiKey;
        }

        protected HttpClient _client;
        protected HttpClient HttpClient
        {
            get
            {
                if (_client == null)
                {
                    _client = new HttpClient();
                    _client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ApiKey);
                }
                return _client;
            }
        }

        public async Task<string> GetAsync(string url)
        {
            return await HttpClient.GetStringAsync(url);
        }

        public async Task<HttpResponseMessage> PostResponseAsync(string url, string param)
        {
            byte[] byteData = Encoding.UTF8.GetBytes(param);
            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                return await HttpClient.PostAsync(url, content);
            }
        }

        public async Task<string> PostAsync(string url, string param)
        {
            var response = await PostResponseAsync(url, param);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<dynamic> GetDynamic(string url)
        {
            var s = await GetAsync(url);
            return Newtonsoft.Json.JsonConvert.DeserializeObject(s);
        }

        public async Task<dynamic> PostDynamic(string url, string param)
        {
            var s = await PostAsync(url,param);
            return Newtonsoft.Json.JsonConvert.DeserializeObject(s);
        }

    }
}
