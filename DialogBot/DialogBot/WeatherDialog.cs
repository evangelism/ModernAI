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

        WeatherParam WP = new WeatherParam();

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<Message> argument)
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
            if (IsPresent(a, "help"))
            {
                return @"This is a simple weather bot.
Example of commands include:
  temperature today
  temperature in Moscow
  humidity tomorrow";
            }
            if (IsPresent(a, "temperature")) WP.MeasurementType = Measurement.Temp;
            if (IsPresent(a, "humidity")) WP.MeasurementType = Measurement.Humidity;
            if (IsPresent(a, "pressure")) WP.MeasurementType = Measurement.Pressure;
            if (IsPresent(a, "today")) { WP.Today(); }
            if (IsPresent(a, "tomorrow")) { WP.Tomorrow(); }
            if (NextTo(a, "in") != "") WP.Location = NextTo(a, "in");
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

        string NextTo(string[] str, string pat)
        {
            for (int i = 0; i < str.Length - 1; i++)
            {
                if (str[i] == pat) return str[i + 1];
            }
            return "";
        }

        bool IsPresent(string[] str, string pat)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == pat) return true;
            }
            return false;
        }


    }
}