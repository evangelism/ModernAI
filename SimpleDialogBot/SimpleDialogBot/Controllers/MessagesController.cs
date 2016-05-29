using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Utilities;
using Newtonsoft.Json;
using MSEvangelism.OpenWeatherMap;
using System.Text;

namespace SimpleDialogBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<Message> Post([FromBody]Message message)
        {
            if (message.Type == "Message")
            {
                var x = message.GetBotUserData<WeatherParam>("weather");
                if (x != null) WP = x;
                var reply = await Reply(message.Text);
                var msg = message.CreateReplyMessage(reply);
                msg.SetBotUserData("weather",WP);
                return msg;
            }
            else
            {
                return HandleSystemMessage(message);
            }
        }

        WeatherParam WP = new WeatherParam();
        WeatherClient OWM = new WeatherClient("88597cb7a556c191905de0f52f23d7d6");

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

        private Message HandleSystemMessage(Message message)
        {
            if (message.Type == "Ping")
            {
                Message reply = message.CreateReplyMessage();
                reply.Type = "Ping";
                return reply;
            }
            else if (message.Type == "DeleteUserData")
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == "BotAddedToConversation")
            {
            }
            else if (message.Type == "BotRemovedFromConversation")
            {
            }
            else if (message.Type == "UserAddedToConversation")
            {
            }
            else if (message.Type == "UserRemovedFromConversation")
            {
            }
            else if (message.Type == "EndOfConversation")
            {
            }

            return null;
        }
    }
}