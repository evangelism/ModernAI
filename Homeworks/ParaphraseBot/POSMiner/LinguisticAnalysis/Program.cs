using Microsoft.ProjectOxford.Linguistics;
using Microsoft.ProjectOxford.Linguistics.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LinguisticAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            Work(args).Wait();
        }

        static Dictionary<string, List<string>> Dict = new Dictionary<string, List<string>>();

        static async Task Work(string[] args)
        { 
            var Client = new LinguisticsClient("96fc9641c5934292be66b62d51fdde5c");

            var Analyzers = await Client.ListAnalyzersAsync();
            var f = File.OpenText(args[0]);
            StringBuilder sb = new StringBuilder();
            int c = 0;

            while (!f.EndOfStream)
            {
                var s = await f.ReadLineAsync();
                if (s.Contains("CHAPTER")||s.Contains("BOOK"))
                {
                    Console.WriteLine(s);
                    c++;
                }
                if (s.Trim()==string.Empty)
                {
                    if (sb.Length > 5)
                    {
                        var Req = new AnalyzeTextRequest();
                        Req.Language = "en";
                        Req.Text = sb.ToString();
                        // Req.AnalyzerIds = (from x in Analyzers select x.Id).ToArray();
                        Req.AnalyzerIds = new Guid[] { Analyzers[1].Id };
                        var Res = await Client.AnalyzeTextAsync(Req);
                        // Console.WriteLine(Res[0].Result);
                        Process(Res[0].Result.ToString());
                        await Task.Delay(1000);
                        Console.Write(".");
                    }
                    sb.Clear();
                }
                else
                {
                    sb.AppendLine(s);
                }
            }
            File.AppendAllText($"{args[0]}.dict",Newtonsoft.Json.JsonConvert.SerializeObject(Dict));
        }

        public static void Process(string s)
        {
            Regex ItemRegex = new Regex(@"\((\w+) (\w+)\)", RegexOptions.Compiled);
            foreach (Match ItemMatch in ItemRegex.Matches(s))
            {
                var x = ItemMatch.Groups[1].ToString();
                var y = ItemMatch.Groups[2].ToString();
                if (!Dict.ContainsKey(x)) Dict.Add(x,new List<string>());
                Dict[x].Add(y);
            }
        }

    }
}
