using Microsoft.ProjectOxford.Emotion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EmoSearch
{
    public partial class _Default : Page
    {
        protected async void Page_Load(object sender, EventArgs e)
        {
            var res = await Search("bill+gates");
            var cli = new EmotionServiceClient("c89be8532536452d996130a6083d3ca5");
            float h = 0, a = 0, s = 0;
            foreach (var x in res)
            {
                var r = await cli.RecognizeAsync(x);
                if (r!=null && r.Length>0)
                {
                    var f = r[0];
                    if (f.Scores.Happiness>h)
                    {
                        h = f.Scores.Happiness;
                        ha.ImageUrl = x;
                    }
                    if (f.Scores.Anger> a)
                    {
                        a = f.Scores.Anger;
                        an.ImageUrl = x;
                    }
                    if (f.Scores.Surprise> s)
                    {
                        s = f.Scores.Surprise;
                        su.ImageUrl = x;
                    }
                    await Task.Delay(1000);
                }
            }
        }

        private async Task<List<string>> Search(string q)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "291131ee5c274fb78a00797c16af69bc");
            var uri = "https://api.cognitive.microsoft.com/bing/v5.0/images/search?count=20&q=" + q;
            var response = await client.GetStringAsync(uri);
            dynamic x = Newtonsoft.Json.JsonConvert.DeserializeObject(response);
            var L = new List<string>();
            foreach (var z in x.value)
            {
                var u = z.contentUrl.ToString();
                L.Add(u);
            }
            return L;
        }


    }
}