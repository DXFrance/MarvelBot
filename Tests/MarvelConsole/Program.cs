namespace MarvelConsole
{
    using MarvelLibrary;
    using System;
    using System.Threading.Tasks;

    public class Program
    {
        public static void Main(string[] args)
        {
            Task.Run(() => Demo());

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
    }
}
