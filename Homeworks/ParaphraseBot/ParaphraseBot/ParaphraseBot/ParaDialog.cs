using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.ProjectOxford.Linguistics;
using Microsoft.ProjectOxford.Linguistics.Contract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace ParaphraseBot
{
    [Serializable]
    public class ParaphraseDialog : IDialog<string>
    {
        public ParaphraseDialog(string p)
        {
            path = p;
        }

        string api_key = "96fc9641c5934292be66b62d51fdde5c";
        string[] stopwords = new string[]{ "i", "do", "does", "have", "been", "not", "if", "then","else" };

        string path;
        Dictionary<string, string[]> Dict;
        Guid AnalyzerId;

        public async Task StartAsync(IDialogContext context)
        {
            if (Dict==null)
            {
                await context.PostAsync("Loading dict wap.dict");
                LoadDict("wap.dict");
                await context.PostAsync("Configuring analyzer");
                AnalyzerId = await GetAnalyzerId();
                await context.PostAsync("Ready to go");
            }
            context.Wait(MessageReceivedAsync);
        }

        void LoadDict(string fn)
        {
            var s = File.ReadAllText(Path.Combine(path,fn));
            Dict = JsonConvert.DeserializeObject<Dictionary<string,string[]>>(s);
        }

        async Task<Guid> GetAnalyzerId()
        {
            var Client = new LinguisticsClient(api_key);
            var Analyzers = await Client.ListAnalyzersAsync();
            return Analyzers[1].Id;
        }

        IDialogContext ctx;

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            ctx = context;
            var message = await argument;
            var repl = await Reply(message.Text);
            await context.PostAsync(repl);
            context.Wait(MessageReceivedAsync);
        }

        async Task<string> Reply(string msg)
        {
            var Client = new LinguisticsClient(api_key);
            var Req = new AnalyzeTextRequest();
            Req.Language = "en";
            Req.Text = msg;
            Req.AnalyzerIds = new Guid[] { AnalyzerId };
            var Res = await Client.AnalyzeTextAsync(Req);
            await ctx.PostAsync(Res[0].Result.ToString());
            return Build(Res[0].Result.ToString());
        }

        Random rnd = new Random();

        string rand(string[] choices)
        {
            return choices[rnd.Next(choices.Length)];
        }

        string Build(string s)
        {
            StringBuilder sb = new StringBuilder();
            Regex ItemRegex = new Regex(@"\((\w+) (\w+)\)", RegexOptions.Compiled);
            foreach (Match ItemMatch in ItemRegex.Matches(s))
            {
                var x = ItemMatch.Groups[1].ToString();
                var y = ItemMatch.Groups[2].ToString();
                if (Dict.ContainsKey(x)&&!stopwords.Contains(y.ToLower()))
                {
                    sb.Append(rand(Dict[x]));
                }
                else
                {
                    sb.Append(y);
                }
                sb.Append(" ");
            }
            return sb.ToString();
        }

    }
}