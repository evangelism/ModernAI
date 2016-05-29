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
            WeatherClient OWM = new WeatherClient("88597cb7a556c191905de0f52f23d7d6");
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
            var res = await OWM.Forecast(WP.Location);
            var r = res[WP.Offset];
            StringBuilder sb = new StringBuilder();
            if (WP.Measure(Measurement.Temp))
            {
                sb.Append($"The temperature on {r.Date} in {WP.Location} is {r.Temp}\r\n");
            }
            if (WP.Measure(Measurement.Pressure))
            {
                sb.Append($"The pressure on {r.Date} in {WP.Location} is {r.Pressure}\r\n");
            }
            if (WP.Measure(Measurement.Humidity))
            {
                sb.Append($"Humidity on {r.Date} in {WP.Location} is {r.Humidity}\r\n");
            }
            if (sb.Length == 0) return "I do not understand";
            else return sb.ToString();
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