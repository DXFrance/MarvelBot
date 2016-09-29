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

            if (activity.Text.ToLower().Contains("hello") || activity.Text.ToLower().Contains("hi"))
            {
                await Conversation.SendAsync(activity, () => new WelcomeDialog());
            }
            else
            {
                await Conversation.SendAsync(activity, () => new SuperHeroInfoDialog());
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }
    }
}