using DigitReco;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DigitInterface
{
    public class AzureClassifier : IClassifier
    {
        public int Classify(int[] value)
        {
            var r = Call(value);
            r.Wait();
            var s = r.Result;
            return 0; 
        }

        public async Task<int> ClassifyAsync(int[] v)
        {
            var res = await Call(v);
            return res;
        }

        public async Task<int> Call(int[] vals)
        {
            var par = new Dictionary<string, string>();
            for (int i = 0; i < vals.Length; i++)
            {
                par.Add($"f{i}", vals[i].ToString());
            }
            var L = new List<Dictionary<string, string>>();
            L.Add(par);
            using (var client = new HttpClient())
            {
                var scoreRequest = new
                {
                    Inputs = new Dictionary<string, List<Dictionary<string, string>>>()
                    {
                        { "digits", L }
                    },
                    GlobalParameters = new Dictionary<string, string>() { }
                };
                const string apiKey = "TC+jq54K+OE8Ne2TAICniiF7jjZGRGCiEVT2UdoUL84tjp6hIWh44DNPMgSbZJui2o23kQL01w0KWOg26rPW5g==";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                client.BaseAddress = new Uri("https://ussouthcentral.services.azureml.net/workspaces/e49a60c938fc4821b8c65716c0a11cfa/services/50382f3cf4a74e1ea16c7973e4e9a312/execute?api-version=2.0&format=swagger");
                HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    dynamic r = JsonConvert.DeserializeObject(result);
                    string s = r.Results.result[0]["Scored Labels"];
                    return int.Parse(s);
                }
                else throw new Exception("Error calling service");


            }
        }

    }
}
