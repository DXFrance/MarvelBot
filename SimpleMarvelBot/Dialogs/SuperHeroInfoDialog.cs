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
            var heroes = await client.GetCharactersAsync(heroName);

            if (heroes.Data.Results.Length == 0)
            {
                await context.PostAsync("I don't know this one.");
            }
            else {
                foreach (var hero in heroes.Data.Results)
                {
                    IMessageActivity replyToConversation = context.MakeMessage();
                    replyToConversation.Type = ActivityTypes.Message;
                    replyToConversation.Attachments = new List<Attachment>();

                    List<CardImage> cardImages = new List<CardImage>();
                    cardImages.Add(new CardImage(url: hero.Thumbnail.Path + "." + hero.Thumbnail.Extension));

                    List<CardAction> cardButtons = new List<CardAction>();
                    CardAction plButton = new CardAction()
                    {
                        Value = hero.Urls[0].Uri,
                        Type = "openUrl",
                        Title = "View more"
                    };
                    cardButtons.Add(plButton);

                    HeroCard plCard = new HeroCard()
                    {
                        Title = hero.Name,
                        Text = hero.Description,
                        Images = cardImages,
                        Buttons = cardButtons
                    };

                    Attachment plAttachment = plCard.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment);

                    await context.PostAsync(replyToConversation);
                }
            }

            context.Wait(MessageReceivedAsync);
        }
    }
}