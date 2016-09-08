using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DialogBotFormFlow
{
    [Serializable]
    public class ChoiceDialog : IDialog<string>
    {
        protected string msg;
        protected List<string> opts = new List<string>();
        public ChoiceDialog(string msg, IEnumerable<string> opts)
        {
            this.msg = msg;
            foreach(var x in opts) { this.opts.Add(x); }
        }
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync(msg);
            var O = new PromptOptions<string>(msg, options: opts);
            PromptDialog.Choice(context, ProcessResult, O);
        }

        private async Task ProcessResult(IDialogContext context, IAwaitable<object> result)
        {
            var res = await result;
            context.Done((string)res);
        }
    }
}