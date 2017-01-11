using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using MSEvangelism.OpenWeatherMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DialogBot.Luis
{
    [Serializable]
    [LuisModel("4a28cd3c-1159-4433-9059-7ebb79b2ffb2", "15a65120ca5347b3b35d8397dde5e480")]
    public class MainDialog : LuisDialog<string>
    {
        [LuisIntent("")]
        public async Task ProcessNone(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("I do not understand");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Hello")]
        public async Task ProcessHello(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Hi!");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Weather")]
        public async Task ProcessWeather(IDialogContext context, LuisResult result)
        {

            var location = "Moscow";
            var measure = "temperature";
            var date = DateTime.Now.Date;

            EntityRecommendation entityContainer;
            if (result.TryFindEntity("builtin.geography.city", out entityContainer))
            {
                location = entityContainer.Entity;
            }

            if (result.TryFindEntity("builtin.datetime.date", out entityContainer))
            {
                DateTime.TryParse(entityContainer?.Resolution?.SingleOrDefault().Value, out date);
            }

            if (result.TryFindEntity("measurement", out entityContainer))
            {
                measure = entityContainer.Entity;
            }

            var weatherClient = new WeatherClient("88597cb7a556c191905de0f52f23d7d6");
            var forecastArray = await weatherClient.Forecast(location);
            var forecast = forecastArray.SingleOrDefault(f => f.When.Date == date.Date);

            string message;
            if (forecast != null)
            {
                if (measure.Contains("humid")) { message = $"The humidity on {forecast.Date} in {location} is {forecast.Humidity}\r\n"; }
                else if (measure.Contains("pres")) { message = $"The pressure on {forecast.Date} in {location} is {forecast.Pressure}\r\n"; }
                else if (measure.Contains("temp") || measure.Contains("cold") || measure.Contains("warm"))
                     { message = $"The temperature on {forecast.Date} in {location} is {forecast.Temp}\r\n"; }
                else { message = "Sorry, unknown parameter \"{parameter}\" requested... Try again"; }
            }
            else { message = "Sorry! I was not able to get the forecast."; }

            await context.PostAsync(message);

            context.Wait(MessageReceived);
        }

    }
}