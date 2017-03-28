using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace FindFaceBot
{
    [Serializable]
    public class FindFaceDialog : IDialog<string>
    {
        string name;

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;            
        }

        public Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            PromptDialog.Text(context, GotName, "Whom do you want to find?");
            return Task.CompletedTask;
        }

        private async Task GotName(IDialogContext context, IAwaitable<string> result)
        {
            name = await result;
            PromptDialog.Choice(context, GotEmo,
                new PromptOptions<string>("Which emotion?", options: new string[] { "Happy", "Sad", "Angry", "Surprise" }));
        }

        private async Task GotEmo(IDialogContext context, IAwaitable<string> result)
        {
            var emo = await result;
            await context.PostAsync("Looking for results, give me some time...");
            var r = await new FaceFinder().Find(name, emo);
            var msg = context.MakeMessage();
            msg.Text = "Look what I found:";
            msg.Attachments.Add(new Attachment(contentUrl: r, contentType: "image/jpeg"));
            await context.PostAsync(msg);
            context.Wait(MessageReceivedAsync);
        }
    }
}