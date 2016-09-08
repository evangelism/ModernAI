using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Evangelism
{

    public class TextAnalysisDocument
    {
        public TextAnalysisDocument() { }
        public TextAnalysisDocument(string id, string lang, string text)
        {
            this.id = id;
            this.language = lang;
            this.text = text;
        }
        public string language { get; set; }
        public string id { get; set; }
        public string text { get; set; }
        public double score { get; set; }
    }

    public class TextAnalysisDocumentStore
    {
        public List<TextAnalysisDocument> documents { get; set; }
        public TextAnalysisDocumentStore()
        {
            documents = new List<Evangelism.TextAnalysisDocument>();
        }

        public TextAnalysisDocumentStore(TextAnalysisDocument doc)
        {
            documents = new List<Evangelism.TextAnalysisDocument>();
            documents.Add(doc);
        }
    }

    public class TextAnalysisClient
    {
        protected string api_key;
        protected string api_uri = "https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/sentiment";


        public TextAnalysisClient(string API_Key)
        {
            api_key = API_Key;
        }

        public async Task<double> Analyze(string text, string lang = "en")
        {
            var T = new TextAnalysisDocumentStore(new TextAnalysisDocument("id",lang,text));
            var R = await AnalyzeRaw(T);
            return R.documents[0].score;
        }

        public async Task<TextAnalysisDocumentStore> AnalyzeRaw(TextAnalysisDocumentStore S)
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
                response = await client.PostAsync(api_uri, content);
                var rstr = await response.Content.ReadAsStringAsync();
                res = Newtonsoft.Json.JsonConvert.DeserializeObject<TextAnalysisDocumentStore>(rstr);
            }
            return res;
        }

        public async Task<TextAnalysisDocumentStore> Analyze(TextAnalysisDocumentStore S)
        {
            var R = await AnalyzeRaw(S);
            for (int i=0;i<R.documents.Count;i++)
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
            return R;
        }
    }
}
