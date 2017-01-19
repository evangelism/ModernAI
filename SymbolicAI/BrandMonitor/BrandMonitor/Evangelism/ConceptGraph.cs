using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Evangelism.ConceptGraph
{
    public class Concept
    {
        public string Name { get; set; }
        public double Probability { get; set; }
    }

    public class ConceptGraphClient
    {
        protected string APIKey { get; set; }
        public ConceptGraphClient(string APIKey)
        {
            this.APIKey = APIKey;
        }

        public virtual async Task<Concept[]> QueryProb(string concept, int topK=10)
        {
            return await QueryInt("ScoreByProb",concept, topK);
        }

        protected async Task<Concept[]> QueryInt(string api, string concept, int topK)
        {
            var http = new HttpClient();
            var res = await http.GetStringAsync($"https://concept.research.microsoft.com/api/Concept/{api}?instance={Uri.EscapeDataString(concept)}&topK={topK}&api_key={APIKey}");
            var jo = JObject.Parse(res);
            var rez = from x in jo.Properties()
                      select new Concept() { Name = x.Name, Probability = x.Value.ToObject<double>() };
            return rez.ToArray();
        }

    }

    public class ConceptGraphCachingClient : ConceptGraphClient
    {
        protected Dictionary<string, Concept[]> dict = new Dictionary<string, Concept[]>();

        public ConceptGraphCachingClient(string APIKey) : base(APIKey)
        {
        }

        public override async Task<Concept[]> QueryProb(string concept, int topK = 10)
        {
            if (dict.ContainsKey(concept)) return dict[concept];
            var x = await base.QueryProb(concept, topK);
            dict[concept] = x;
            return x;
        }

    }

}
