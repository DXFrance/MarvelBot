namespace MarvelConsole
{
    using MarvelLibrary;
    using Microsoft.Cognitive.Face;
    using Microsoft.Cognitive.Face.Contract;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class Program
    {
        public static void Main(string[] args)
        {
            //Task.Run(() => Demo());
            //Task.Run(() => Training());
            Task.Run(() => Identify());

            Console.ReadLine();
        }

        public static async void Demo()
        {
            string publicKey = "a80f430c49270656a6568f3e83b32263";
            string privateKey = "a0c1bdb3674715f80067fe4226464c1585c69ba1";

            IMarvelClient proxy = MarvelClientFactory.CreateMarvelClient(publicKey, privateKey);
            var characters = await proxy.GetCharactersAsync("DareDevil");

            Console.WriteLine(characters.Copyright);

            foreach (var result in characters.Data.Results)
            {
                Console.WriteLine(result.Name);
                Console.WriteLine(result.Description);
            }

            var comics = await proxy.GetComicsAsync(new DateTime(2016, 6, 1), new DateTime(2016, 6, 2));

            Console.WriteLine(comics.Copyright);

            foreach (var result in comics.Data.Results)
            {
                Console.WriteLine(result.Title);
            }
        }

        public static async void Training()
        {
            var subscriptionKey = "84bdb4c2391a4f3bb212d06570446ee4";
            var faceServiceClient = new FaceServiceClient(subscriptionKey);
            var personGroupId = "28d8a343-0790-4029-83e6-a0dfb39f5be5";
            var personGroupName = "Marvel";

            bool groupExists = false;

            try
            {
                await faceServiceClient.GetPersonGroupAsync(personGroupId);
                groupExists = true;
            }
            catch (FaceAPIException ex)
            {

                if (ex.ErrorCode != "PersonGroupNotFound")
                {
                    Debug.WriteLine("Response: {0}. {1}", ex.ErrorCode, ex.ErrorMessage);
                    return;
                }
                else
                {
                    Debug.WriteLine("Response: Group {0} did not exist previously.", personGroupId);
                }
            }

            if (groupExists)
            {
                await faceServiceClient.DeletePersonGroupAsync(personGroupId);
                Console.WriteLine("Existing group deleted.");
            }

            try
            {
                await faceServiceClient.CreatePersonGroupAsync(personGroupId, personGroupName);
                Console.WriteLine("Person group created.");
            }
            catch (FaceAPIException ex)
            {
                Debug.WriteLine(ex.Message);
            }

            foreach (var dir in System.IO.Directory.EnumerateDirectories(@"..\..\Assets"))
            {
                var tasks = new List<Task>();
                var tag = System.IO.Path.GetFileName(dir);

                Person p = new Person();
                p.Name = tag;
                p.PersonId = (await faceServiceClient.CreatePersonAsync(personGroupId, p.Name)).PersonId;

                Console.WriteLine($"Person {p.Name} created.");

                string img;

                // Enumerate images under the person folder, call detection
                var imageList =
                new ConcurrentBag<string>(
                    Directory.EnumerateFiles(dir, "*.*", SearchOption.AllDirectories)
                        .Where(s => s.EndsWith(".jpg") || s.EndsWith(".png") || s.EndsWith(".bmp") || s.EndsWith(".gif")));


                while (imageList.TryTake(out img))
                {
                    var imgPath = img;
                    using (var fStream = File.OpenRead(imgPath))
                    {
                        try
                        {
                            // Update person faces on server side
                            var persistFace = await faceServiceClient.AddPersonFaceAsync(personGroupId, p.PersonId, fStream, imgPath);

                            Console.WriteLine($"Adding {imgPath}");
                        }
                        catch (FaceAPIException ex)
                        {
                            Debug.WriteLine("Response: {0}. {1}", ex.ErrorCode, ex.ErrorMessage);
                        }
                    }
                }
            }
        
            try
            {
                // Start train person group
                await faceServiceClient.TrainPersonGroupAsync(personGroupId);

                // Wait until train completed
                while (true)
                {
                    await Task.Delay(1000);

                    var status = await faceServiceClient.GetPersonGroupTrainingStatusAsync(personGroupId);

                    if (status.Status != Status.Running)
                    {
                        break;
                    }
                }

                Console.WriteLine($"Training completed");
            }
            catch (FaceAPIException ex)
            {
                Debug.WriteLine("Response: {0}. {1}", ex.ErrorCode, ex.ErrorMessage);
            }
        }

        public static async void Identify()
        {
            var subscriptionKey = "84bdb4c2391a4f3bb212d06570446ee4";
            var faceServiceClient = new FaceServiceClient(subscriptionKey);

            var personGroupId = "28d8a343-0790-4029-83e6-a0dfb39f5be5";

            using (var fileStream = File.OpenRead(@"..\..\Assets\avengers.jpg"))
            {
                try
                {
                    var faces = await faceServiceClient.DetectAsync(fileStream);

                    var identifyResult = await faceServiceClient.IdentifyAsync(personGroupId, faces.Select(ff => ff.FaceId).ToArray());

                    foreach (var result in identifyResult)
                    {
                        foreach (var candidate in result.Candidates)
                        { 
                            var person = await faceServiceClient.GetPersonAsync(personGroupId, candidate.PersonId);
                            Console.WriteLine($"{person.Name}");
                        }
                    }
                }
                catch(FaceAPIException ex)
                {
                    Debug.WriteLine("Response: {0}. {1}", ex.ErrorCode, ex.ErrorMessage);
                }
            }
        }
    }
}
