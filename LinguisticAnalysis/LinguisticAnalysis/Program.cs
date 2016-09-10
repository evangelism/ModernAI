using Microsoft.ProjectOxford.Linguistics;
using Microsoft.ProjectOxford.Linguistics.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinguisticAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            Work().Wait();
        }
        static async Task Work()
        { 
            var Client = new LinguisticsClient("96fc9641c5934292be66b62d51fdde5c");

            var Analyzers = await Client.ListAnalyzersAsync();
            Console.WriteLine("ANALYZERS");

            foreach (var a in Analyzers)
            {
                Console.WriteLine($" > {a.Implementation}");
            }

            var f = File.OpenText(@"Data\wap.txt");
            StringBuilder sb = new StringBuilder();
            int c = 0;

            while (!f.EndOfStream)
            {
                var s = await f.ReadLineAsync();
                if (s.Contains("CHAPTER")||s.Contains("BOOK"))
                {
                    Console.WriteLine(s);
                    c++;
                    if (c > 10) break;
                    continue;
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
                        Console.WriteLine(Res[0].Result);
                        Console.ReadKey();
                    }
                    sb.Clear();
                }
                else
                {
                    sb.AppendLine(s);
                }
            }
        }
    }
}
