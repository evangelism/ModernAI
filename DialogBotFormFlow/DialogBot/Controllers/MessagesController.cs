using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using System.Collections.Generic;

namespace DialogBot
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
                // return our reply to the user
                return await Conversation.SendAsync(message, () =>
                FormDialog.FromForm(WeatherParam.BuildForm)
                .ContinueWith<WeatherParam, string>(async (ctx, res) =>
                 {
                     var WP = await res;
                     var s = await WP.BuildResult();
                     await ctx.PostAsync(s);
                     return new ChoiceDialog($"Do you want to subscribe to weather in {WP.Location}?", new string[] { "Yes", "No" });
                 }).Do(async (ctx, res) =>
                 {
                     var r = await res;
                     if (r.ToLower()=="yes")
                     {
                         await ctx.PostAsync("Subscribed");
                     }
                     else
                     {
                         await ctx.PostAsync("Not subscribed");
                     }
                 }));
            }
            else
            {
                return HandleSystemMessage(message);
            }
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