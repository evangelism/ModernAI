using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Evangelism.Cognitive.CustomVision;
using System.Net.Http;

namespace CustomVisionBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        string ModelId = "7d906c46-3990-49fb-aaa7-3aae1f64be56";
        string PredictionKey = "2b45dbf8490941baa90b0d2d698af104";

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            if (activity.Attachments.Count > 0)
            {
                if (ModelId =="" || PredictionKey=="")
                {
                    await context.PostAsync("Please specify ModelID and Key before use");
                }
                else
                {
                    var cli = new CustomVisionClient(ModelId, PredictionKey);
                    var hcli = new HttpClient();
                    var str = await hcli.GetStreamAsync(activity.Attachments[0].ContentUrl);
                    var res = await cli.AnalyseAsync(str);
                    var top = res.Top;
                    if (top.Probability < 0)
                    {
                        await context.PostAsync("No object found");
                    }
                    else
                    {
                        await context.PostAsync($"I found brand {top.Name} with confidence={top.Probability}");
                    }
                }
            }
            else
            {
                var t = activity.Text.Split(' ');
                if (t.Length != 2)
                {
                    await context.PostAsync("Please specify ModelId and Key separated by space");
                }
                else
                {
                    ModelId = t[0];
                    PredictionKey = t[1];
                }
            }
            context.Wait(MessageReceivedAsync);
        }
    }
}