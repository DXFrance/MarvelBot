using MarvelLibrary;
using Microsoft.Bot.Connector;
using Microsoft.Cognitive.Face;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
            var tempImageFolder = Path.Combine(Path.GetTempPath(), "totest.jpg");

            var personGroupId = "28d8a343-0790-4029-83e6-a0dfb39f5be5";

            var data = await GetAttachmentsAsByteArrayAsync(activity);
            File.WriteAllBytes(tempImageFolder, data.First<byte[]>());

            Activity carouselActivity = activity.CreateReply();
            carouselActivity.Type = ActivityTypes.Message;
            carouselActivity.Attachments = new List<Attachment>();
            carouselActivity.AttachmentLayout = AttachmentLayoutTypes.Carousel;

            using (var fileStream = File.OpenRead(tempImageFolder))
            {
                try
                {
                    var faces = await faceServiceClient.DetectAsync(fileStream);
                    if (faces.Length > 0)
                    {
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
                }
                catch (FaceAPIException ex)
                {
                    Debug.WriteLine("Response: {0}. {1}", ex.ErrorCode, ex.ErrorMessage);
                }
                
                return carouselActivity;
            }
        }

        private static async Task<IEnumerable<byte[]>> GetAttachmentsAsByteArrayAsync(Activity activity)
        {
            var attachments = activity?.Attachments?
            .Where(attachment => attachment.ContentUrl != null)
            .Select(c => Tuple.Create(c.ContentType, c.ContentUrl));
            if (attachments != null && attachments.Any())
            {
                var contentBytes = new List<byte[]>();
                using (var connectorClient = new ConnectorClient(new Uri(activity.ServiceUrl)))
                {
                    var token = await (connectorClient.Credentials as MicrosoftAppCredentials).GetTokenAsync();
                    foreach (var content in attachments)
                    {
                        var uri = new Uri(content.Item2);
                        using (var httpClient = new HttpClient())
                        {
                            if (uri.Host.EndsWith("skype.com") && uri.Scheme == "https")
                            {
                                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));
                            }
                            else
                            {
                                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(content.Item1));
                            }
                            contentBytes.Add(await httpClient.GetByteArrayAsync(uri));
                        }
                    }
                }
                return contentBytes;
            }
            return null;
        }
    }
}