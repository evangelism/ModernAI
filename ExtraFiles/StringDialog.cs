using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MyBot
{
    [Serializable]
    public class StringDialog : IDialog<string>
    {
        protected string msg;
        public StringDialog(string msg)
        {
            this.msg = msg;
        }
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync(msg);
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            context.Done(message.Text);
        }

    }
}