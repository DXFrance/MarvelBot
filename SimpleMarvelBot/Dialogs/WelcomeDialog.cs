using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SimpleMarvelBot.Dialogs
{
    [Serializable]
    public class WelcomeDialog:IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            await context.PostAsync("Hello ! I am the **Simple Marvel Bot**. You can ask me the detail of Marvel super heroes. For instance, you can say: '*I want to know about a hero*'.");
            context.Wait(MessageReceivedAsync);
        }
    }
}