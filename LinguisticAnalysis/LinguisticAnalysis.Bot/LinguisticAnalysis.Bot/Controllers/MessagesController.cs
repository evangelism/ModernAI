using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Microsoft.ProjectOxford.Linguistics;
using System.Text;
using Microsoft.ProjectOxford.Linguistics.Contract;
using System.Text.RegularExpressions;

namespace LinguisticAnalysis.Bot
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

                await Reply(activity.Text,
                            async s =>
                            {
                                Activity reply = activity.CreateReply(s);
                                await connector.Conversations.ReplyToActivityAsync(reply);
                            });
                }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        async Task Reply(string s, Func<string, Task> reply)
        {
            var Client = new LinguisticsClient("202b0a9b3ad749d9bffcc77800124268");
            var Analyzers = await Client.ListAnalyzersAsync();
            var Req = new AnalyzeTextRequest();
            Req.AnalyzerIds = (from x in Analyzers
                               where x.Kind == "Constituency_Tree"
                               select x.Id).ToArray();
            Req.Text = s;
            Req.Language = "en";
            var Resp = await Client.AnalyzeTextAsync(Req);
            await reply(Resp[0].Result.ToString());
            var r = Process(Resp[0].Result.ToString(), (a, b) =>
              {
                  switch (a)
                  {
                      case "NN":
                          return "bot";
                      case "NNS":
                          return "bots";
                      case "JJ":
                          return "crazy";
                      case "PRP":
                          if (b.ToLower() == "i") return "you";
                          else if (b.ToLower() == "you") return "I";
                          else return b;
                      default:
                          return b;
                  }
              });
            await reply(r);
        }

        string Process(string s, Func<string,string,string> P)
        {
            var sb = new StringBuilder();
            Regex ItemRegex = new Regex(@"\((\w+) (\w+)\)", RegexOptions.Compiled);
            foreach (Match ItemMatch in ItemRegex.Matches(s))
            {
                sb.Append(P(ItemMatch.Groups[1].ToString(), ItemMatch.Groups[2].ToString()));
                sb.Append(" ");
            }
            return sb.ToString();
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