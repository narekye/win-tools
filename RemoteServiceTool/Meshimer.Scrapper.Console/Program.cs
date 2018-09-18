using Meshimer.Scrapper.BLL;
using System.Linq;

namespace Meshimer.Scrapper.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            BrowserTypeEnum browser = BrowserTypeEnum.Chrome;
            if (args != null && args.Any())
            {
                var browserName = args[0];
                switch (browserName.ToLower())
                {
                    case "firefox":
                        browser = BrowserTypeEnum.Firefox;
                        break;
                    default:
                        break;
                }
            }

            string username = string.Empty;

            using (var scrapper = new MeshimerScrapper(browser))
            {
                username = scrapper.GetUsernameFromMeshimerPage();
            }

            System.Console.Clear();
            System.Console.WriteLine("Username from Meshimer page {0}", username);
            System.Console.Read();
        }
    }
}
