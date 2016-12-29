using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using MSEvangelism.OpenWeatherMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DialogBot
{
    [Serializable]
    public class WeatherDialog : IDialog<WeatherParam>
    {

        WeatherParam WP;

        public async Task StartAsync(IDialogContext context)
        {
            if (WP == null) WP = new WeatherParam();
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;

            if (message.Text.ToLower().Contains("subscribe"))
            {
                PromptDialog.Confirm(context, Subscribe,
                    $"Do you want to subscribe to weather into in {WP.Location}?");
            }
            else
            {
                var repl = await Reply(message.Text);
                await context.PostAsync(repl);
                context.Wait(MessageReceivedAsync);
            }
        }

        async Task<string> Reply(string msg)
        {
            var a = msg.ToLower().Split(' ');
            if (a.Contains("help"))
            {
                return @"This is a simple weather bot.
Example of commands include:
  temperature today
  temperature in Moscow
  humidity tomorrow";
            }
            if (a.Contains("temperature")) WP.MeasurementType = Measurement.Temp;
            if (a.Contains("humidity")) WP.MeasurementType = Measurement.Humidity;
            if (a.Contains("pressure")) WP.MeasurementType = Measurement.Pressure;
            if (a.Contains("today")) { WP.Today(); }
            if (a.Contains("tomorrow")) { WP.Tomorrow(); }
            if (a.NextTo("in") != "") WP.Location = a.NextTo("in");
            return await WP.BuildResult();
        }

        private async Task Subscribe(IDialogContext context, IAwaitable<bool> result)
        {
            var ans = await result;
            if (ans)
            {
                await context.PostAsync("You are subscribed");
            }
            else
            {
                await context.PostAsync("Subscription cancelled");
            }
            context.Wait(MessageReceivedAsync);
        }
    }
}