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
using SimpleMarvelBot.Dialogs;
using System.Text.RegularExpressions;

namespace SimpleMarvelBot
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
            ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            if (activity.Type == ActivityTypes.Message)
            {
                if (activity.Text.ToLower().Contains("hello") || activity.Text.ToLower().Contains("hi"))
                {
                    var reply = activity.CreateReply($"Hello {activity.From.Name} ! I am the **Simple Marvel Bot**. You can ask me the detail of Marvel super heroes. For instance, you can say: '*I want to know about a hero*'.");
                    await connector.Conversations.ReplyToActivityAsync(reply);
                }
                else
                {
                    await Conversation.SendAsync(activity, () => new SuperHeroInfoDialog());
                }
            }
            else if(activity.Type == ActivityTypes.ConversationUpdate && activity. .Count > 0)
            {
                var reply = activity.CreateReply("Say hello !");
                await connector.Conversations.ReplyToActivityAsync(reply);
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }
    }
}