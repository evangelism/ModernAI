using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace Evangelism.Cognitive.CustomVision
{
    public class TagInfo
    {
        public string Name { get; set; }
        public float Probability { get; set; }
    }

    public class TagCollection : List<TagInfo>
    {
        public TagInfo Top
        {
            get
            {
                return (from z in this orderby z.Probability descending select z).FirstOrDefault();
            }    
        }
    }

    public class CustomVisionClient
    {
        public string PredictionKey { get; set; }
        public string ModelId { get; set; }
        public CustomVisionClient(string ModelId, string PredictionKey)
        {
            this.ModelId = ModelId;
            this.PredictionKey = PredictionKey;
        }

        public async Task<TagCollection> AnalyseAsync(Stream str)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Prediction-Key", PredictionKey);

            string url = $"https://southcentralus.api.cognitive.microsoft.com/customvision/v1.0/prediction/{ModelId}/image";

            HttpResponseMessage response;
            string res;
            using (var content = new StreamContent(str))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(url, content);
                res = await response.Content.ReadAsStringAsync();
            }
            dynamic z = Newtonsoft.Json.JsonConvert.DeserializeObject(res);
            var r = new TagCollection();
            foreach(var x in z.Predictions)
            {
                r.Add(new TagInfo() { Name = x.Tag, Probability = x.Probability });
            }
            return r;
        }
    }
}