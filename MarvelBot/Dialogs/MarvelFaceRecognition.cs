using MarvelLibrary;
using Microsoft.Bot.Connector;
using Microsoft.Cognitive.Face;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace MarvelBot.Dialogs
{
    public static class MarvelFaceRecognition
    {
        public static async Task<Activity> Identify(string url, Activity activity)
        {
            var subscriptionKey = "84bdb4c2391a4f3bb212d06570446ee4";
            var faceServiceClient = new FaceServiceClient(subscriptionKey);
            var tempImageFolder = @"c:\images\totest.jpg";

            var personGroupId = "28d8a343-0790-4029-83e6-a0dfb39f5be5";

            byte[] data;
            using (WebClient client = new WebClient())
            {
                data = client.DownloadData(url);
            }
            File.WriteAllBytes(tempImageFolder, data);

            Activity carouselActivity = activity.CreateReply();
            carouselActivity.Type = ActivityTypes.Message;
            carouselActivity.Attachments = new List<Attachment>();
            carouselActivity.AttachmentLayout = AttachmentLayoutTypes.Carousel;

            using (var fileStream = File.OpenRead(tempImageFolder))
            {
                try
                {
                    var faces = await faceServiceClient.DetectAsync(fileStream);
                    var identifyResult = await faceServiceClient.IdentifyAsync(personGroupId, faces.Select(ff => ff.FaceId).ToArray());
                    var client = MarvelClientFactory.CreateMarvelClient("4304d8f80441726f68ce32b2819c3b91", "b8c5e4506efa790150a1a4d16920048159d16794");

                    foreach (var result in identifyResult)
                    {
                        foreach (var candidate in result.Candidates)
                        {
                            var heroName = await faceServiceClient.GetPersonAsync(personGroupId, candidate.PersonId);

                            //Create hero card
                            var heroes = await client.GetCharactersAsync(heroName.Name);

                            foreach (var hero in heroes.Data.Results)
                            {
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
                                carouselActivity.Attachments.Add(plAttachment);
                            }
                        }
                    }
                }
                catch (FaceAPIException ex)
                {
                    Debug.WriteLine("Response: {0}. {1}", ex.ErrorCode, ex.ErrorMessage);
                }

                carouselActivity.Text = $"I found {carouselActivity.Attachments.Count} heroes in the picture.";

                return carouselActivity;
            }
        }
    }
}