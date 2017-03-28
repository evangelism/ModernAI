using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace FindFaceBot
{
    public class BingImageSearch
    {
        protected string key = "";
        public BingImageSearch(string Key)
        {
            key = Key;
        }

        public async Task<IEnumerable<string>> Search(string q, int no=20)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);
            var uri = $"https://api.cognitive.microsoft.com/bing/v5.0/images/search?count={no}&q={System.Web.HttpUtility.UrlEncode(q)}";
            var response = await client.GetStringAsync(uri);
            dynamic x = Newtonsoft.Json.JsonConvert.DeserializeObject(response);
            var L = new List<string>();
            foreach (var z in x.value)
            {
                L.Add(z.contentUrl.ToString());
            }
            return L;
        }


    }
}