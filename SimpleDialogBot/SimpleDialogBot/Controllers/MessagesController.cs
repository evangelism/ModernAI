using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace SimpleDialogBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                var State = activity.GetStateClient();
                var UserData = await State.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);
                var x = UserData.GetProperty<WeatherParam>("weather");
                if (x != null) WP = x;
                var rep = await Reply(activity.Text);
                Activity reply = activity.CreateReply(rep);
                UserData.SetProperty<WeatherParam>("weather", WP);
                await State.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, UserData);
                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        WeatherParam WP = new WeatherParam();

        async Task<string> Reply(string msg)
        {
            var a = msg.ToLower().Split(' ');
            if (a.IsPresent("help"))
            {
                return @"This is a simple weather bot.
Example of commands include:
  temperature today
  temperature in Moscow
  humidity tomorrow";
            }
            if (a.IsPresent("temperature")) WP.MeasurementType = Measurement.Temp;
            if (a.IsPresent("humidity")) WP.MeasurementType = Measurement.Humidity;
            if (a.IsPresent("pressure")) WP.MeasurementType = Measurement.Pressure;
            if (a.IsPresent("today")) { WP.Today(); }
            if (a.IsPresent("tomorrow")) { WP.Tomorrow(); }
            if (a.NextTo("in") != "") WP.Location = a.NextTo("in");
            return await WP.BuildResult();
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}