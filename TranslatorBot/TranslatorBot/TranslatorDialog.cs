using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using MSEvangelism.BingTranslator;

namespace TranslatorBot
{
    [Serializable]
    public class TranslatorDialog : IDialog<string>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }
        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            var tr = new BingTranslatorClient(Config.TranslatorKey, Config.TranslatorSecret);
            var res = await tr.Translate(message.Text, "ru-RU", "en-US");
            await context.PostAsync(res);
            context.Wait(MessageReceivedAsync);
        }


    }
}