using MarvelLibrary;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MarvelBot.Dialogs
{
    [LuisModel("3ae5f884-a77d-4af4-aa23-b32b28d63e24", "a48f97a5db7b4bc98da1c2bb11c473de")]
    [Serializable]
    public class MarvelLuisDialog : LuisDialog<object>
    {
        [LuisIntent("Intent.Hello")]
        public async Task Hello(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Hello, I am the **Marvel Bot**! You can get information about Marvel super heroes and comics. Try saying '*Do you know Daredevil?*'");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Intent.Help")]
        public async Task Help(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Here are some questions you can ask me: {Environment.NewLine}- *Do you know iron man?* {Environment.NewLine}- *do you have information about jessica jones ?* {Environment.NewLine}- *which comics were released between 12 july 2016 till 26 july 2016 ?* ");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Intent.GetCharacterInfo")]
        public async Task GetCharacterInfo(IDialogContext context, LuisResult result)
        {
            var hero = "";
            EntityRecommendation character;
            if (result.TryFindEntity("Entity.Character", out character))
            {
                hero = character.Entity;
                await SendHeroCard(context, hero);
            }
            else
            {
                await context.PostAsync("Did not understand which character (getcharacterinfo)");
            }

            context.Wait(MessageReceived);
        }

        [LuisIntent("Intent.GetComics")]
        public async Task GetComics(IDialogContext context, LuisResult result)
        {
            DateTime startdate, enddate;
            EntityRecommendation startdateEntity, enddateEntity;
            if (result.TryFindEntity("Entity.Date::StartDate", out startdateEntity) && result.TryFindEntity("Entity.Date::EndDate", out enddateEntity))
            {
                startdate = DateTime.Parse(startdateEntity.Entity);
                enddate = DateTime.Parse(enddateEntity.Entity);

                await SendComicsCarousel(context, startdate, enddate);
            }
            else
            {
                await context.PostAsync("Did not understand dates (getcomics)");
            }

            context.Wait(MessageReceived);
        }

        #region Helpers

        public async Task SendHeroCard(IDialogContext context, string heroName)
        {
            var client = MarvelClientFactory.CreateMarvelClient("4304d8f80441726f68ce32b2819c3b91", "b8c5e4506efa790150a1a4d16920048159d16794");
            var heroes = await client.GetCharactersAsync(heroName);

            if (heroes.Data.Results.Length == 0)
            {
                await context.PostAsync("I don't know this one.");
            }
            else
            {
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
                        Value = hero.Urls[0].Uri.Replace("http", "https"),
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
        }

        public async Task SendComicsCarousel(IDialogContext context, DateTime from, DateTime to)
        {
            var client = MarvelClientFactory.CreateMarvelClient("4304d8f80441726f68ce32b2819c3b91", "b8c5e4506efa790150a1a4d16920048159d16794");
            var comics = await client.GetComicsAsync(from, to);

            if (comics.Data.Results.Length == 0)
            {
                await context.PostAsync("I did not find any.");
            }
            else
            {
                IMessageActivity replyToConversation = context.MakeMessage();
                replyToConversation.Type = ActivityTypes.Message;
                replyToConversation.Attachments = new List<Attachment>();
                replyToConversation.AttachmentLayout = AttachmentLayoutTypes.Carousel;

                foreach (var comic in comics.Data.Results)
                {
                    if (string.IsNullOrEmpty(comic.Description))
                        continue;

                    List<CardImage> cardImages = new List<CardImage>();
                    cardImages.Add(new CardImage(url: comic.Thumbnail.Path + "." + comic.Thumbnail.Extension));

                    List<CardAction> cardButtons = new List<CardAction>();
                    CardAction plButton = new CardAction()
                    {
                        Value = comic.urls[0].Uri.Replace("http", "https"),
                        Type = "openUrl",
                        Title = "View more"
                    };
                    cardButtons.Add(plButton);

                    HeroCard plCard = new HeroCard()
                    {
                        Title = comic.Title,
                        Text = comic.Description,
                        Images = cardImages,
                        Buttons = cardButtons
                    };

                    Attachment plAttachment = plCard.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment);
                }

                await context.PostAsync(replyToConversation);
            }
        }

        #endregion
    }
}