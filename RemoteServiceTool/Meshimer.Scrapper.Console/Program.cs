using Meshimer.Scrapper.BLL;

namespace Meshimer.Scrapper.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            string username = string.Empty;

            using (var scrapper = new MeshimerScrapper(BrowserTypeEnum.Chrome))
            {
                username = scrapper.GetUsernameFromMeshimerPage();
            }

            System.Console.Clear();
            System.Console.WriteLine("Username from Meshimer page {0}", username);
            System.Console.Read();
        }
    }
}
