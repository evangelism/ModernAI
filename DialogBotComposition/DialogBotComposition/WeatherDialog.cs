using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using MSEvangelism.OpenWeatherMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DialogBotComposition
{
    [Serializable]
    public class WeatherDialog : IDialog<WeatherParam>
    {

        WeatherParam WP = new WeatherParam();

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            if (message.Text.ToLower().Contains("done"))
            {
                context.Done(WP);
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
    }
}