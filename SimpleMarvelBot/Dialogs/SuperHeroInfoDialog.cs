using MarvelLibrary;
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
    public class SuperHeroInfoDialog: IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            PromptDialog.Text(
                    context,
                    HeroNameGiven,
                    "Sure! What is the hero name you want to know more about?");
        }

        public async Task HeroNameGiven(IDialogContext context, IAwaitable<string> argument)
        {
            var heroName = await argument;

            var client = MarvelClientFactory.CreateMarvelClient("4304d8f80441726f68ce32b2819c3b91", "b8c5e4506efa790150a1a4d16920048159d16794");
            var hero = await client.GetCharactersAsync(heroName);

            await context.PostAsync($"I found {hero.Data.Count} heroe(s).");
            
            context.Wait(MessageReceivedAsync);
        }
    }
}