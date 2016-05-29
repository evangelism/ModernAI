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
using System.Text;
using MSEvangelism.OpenWeatherMap;

namespace SimpleCommandBot
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
                var reply = await Reply(message.Text);
                return message.CreateReplyMessage(reply);
            }
            else
            {
                return HandleSystemMessage(message);
            }
        }



        string NextTo(string[] str, string pat)
        {
            for (int i=0;i<str.Length-1;i++)
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

        enum Measurement { Temp=1, Humidity=2, Pressure=4, None=0 };

        WeatherClient OWM = new WeatherClient("88597cb7a556c191905de0f52f23d7d6");

        async Task<string> Reply(string msg)
        {
            string city = "Moscow";
            int when = 0;
            string whens = "today";
            Measurement mes = Measurement.None;
            var a = msg.ToLower().Split(' ');
            if (IsPresent(a,"help"))
            {
                return @"This is a simple weather bot.
Example of commands include:
  temperature today
  temperature in Moscow
  humidity tomorrow";
            }
            if (IsPresent(a, "temperature")) mes |= Measurement.Temp;
            if (IsPresent(a, "humidity")) mes |= Measurement.Humidity;
            if (IsPresent(a, "pressure")) mes |= Measurement.Pressure;
            if (IsPresent(a, "today")) { when = 0; whens = "today"; }
            if (IsPresent(a, "tomorrow")) { when = 1; whens = "tomorrow"; }
            if (NextTo(a, "in") != "") city = NextTo(a, "in");
            var res = await OWM.Forecast(city);
            var r = res[when];
            StringBuilder sb = new StringBuilder();
            if ((mes & Measurement.Temp)>0)
            {
                sb.Append($"The temperature on {r.Date} in {city} is {r.Temp}\r\n"); 
            }
            if ((mes & Measurement.Pressure) > 0)
            {
                sb.Append($"The pressure on {r.Date} in {city} is {r.Pressure}\r\n");
            }
            if ((mes & Measurement.Humidity) > 0)
            {
                sb.Append($"Humidity on {r.Date} in {city} is {r.Humidity}\r\n");
            }
            if (sb.Length == 0) return "I do not understand";
            else return sb.ToString();
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