using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Net.Http;
using Microsoft.ProjectOxford.Face;

namespace AgeBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var msg = await result as Activity;

            if (msg.Attachments.Count > 0)
            {
                var cli = new FaceServiceClient("1a9025a2ec4d43928e60f90cd5db6002");
                var hcli = new HttpClient();
                var str = await hcli.GetStreamAsync(msg.Attachments[0].ContentUrl);
                var res = await cli.DetectAsync(str,returnFaceAttributes: new FaceAttributeType[]{ FaceAttributeType.Age, FaceAttributeType.Gender });
                if (res.Length > 0)
                {
                }
                else await context.PostAsync("No faces");
            }
            else await context.PostAsync("No pics :(");

            context.Wait(MessageReceivedAsync);
        }
    }
}