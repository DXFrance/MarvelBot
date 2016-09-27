namespace MarvelConsole
{
    using MarvelLibrary;
    using System;
    using System.Threading.Tasks;

    public class Program
    {
        public static void Main(string[] args)
        {
            Task.Run(() => GetCharacterDetails());

            Console.ReadLine();
        }

        public static async void GetCharacterDetails()
        {
            string publicKey = "a80f430c49270656a6568f3e83b32263";
            string privateKey = "a0c1bdb3674715f80067fe4226464c1585c69ba1";

            IMarvelClient proxy = MarvelClientFactory.CreateMarvelClient(publicKey, privateKey);
            var response = await proxy.GetCharactersAsync("DareDevil");

            Console.WriteLine(response.Copyright);

            foreach (var result in response.Data.Results)
            {
                Console.WriteLine(result.Name);
                Console.WriteLine(result.Description);
            }
        }
    }
}
