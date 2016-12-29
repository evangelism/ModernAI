using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Evangelism
{
    
    public class TextAnalysisClient
    {
        protected string api_key;
        protected string api_uri_sentiment = "https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/sentiment";
        protected string api_uri_keyphrases = "https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/keyPhrases";


        public TextAnalysisClient(string API_Key)
        {
            api_key = API_Key;
        }

        public async Task<double> AnalyzeSentiment(string text, string lang = "en")
        {
            var T = new TextAnalysisDocumentStore(new TextAnalysisDocument("id",lang,text));
            var R = await AnalyzeSentimentRaw(T);
            return R.documents[0].score;
        }

        public async Task<TextAnalysisDocumentStore> AnalyzeSentimentRaw(TextAnalysisDocumentStore S)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", api_key);

            HttpResponseMessage response;
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(S);
            byte[] byteData = Encoding.UTF8.GetBytes(s);

            TextAnalysisDocumentStore res;

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(api_uri_sentiment, content);
                var rstr = await response.Content.ReadAsStringAsync();
                res = Newtonsoft.Json.JsonConvert.DeserializeObject<TextAnalysisDocumentStore>(rstr);
            }
            return res;
        }

        public async Task<TextAnalysisDocumentStore> AnalyzeSentiment(TextAnalysisDocumentStore S)
        {
            var R = await AnalyzeSentimentRaw(S);
            CopyDocumentInfo(S, R);
            return R;
        }

        public async Task<TextAnalysisDocumentStore> ExtractKeyphrases(TextAnalysisDocumentStore S)
        {
            var R = await ExtractKeyPhrasesRaw(S);
            CopyDocumentInfo(S, R);
            return R;
        }

        private static void CopyDocumentInfo(TextAnalysisDocumentStore S, TextAnalysisDocumentStore R)
        {
            for (int i = 0; i < R.documents.Count; i++)
            {
                var t = (from x in S.documents
                         where x.id == R.documents[i].id
                         select x).FirstOrDefault();
                if (t != null)
                {
                    R.documents[i].text = t.text;
                    R.documents[i].language = t.language;
                }
            }
        }

        public async Task<TextAnalysisDocumentStore> ExtractKeyPhrasesRaw(TextAnalysisDocumentStore S)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", api_key);

            HttpResponseMessage response;
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(S);
            byte[] byteData = Encoding.UTF8.GetBytes(s);

            TextAnalysisDocumentStore res;

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(api_uri_keyphrases, content);
                var rstr = await response.Content.ReadAsStringAsync();
                res = Newtonsoft.Json.JsonConvert.DeserializeObject<TextAnalysisDocumentStore>(rstr);
            }
            return res;
        }


    }
}
