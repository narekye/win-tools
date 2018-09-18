using Meshimer.Common;
using Meshimer.Common.Logger;
using Meshimer.Scrapper.BLL;
using System.Linq;
using System.Threading;

namespace Meshimer.Scrapper.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            // -browser [chrome] [firefox]
            BrowserTypeEnum browser = BrowserTypeEnum.Chrome;

            Logger.Instance.LogMessage("App started");

            if (args != null && args.Any())
            {
                var browserName = args[1].Trim();
                if (!string.IsNullOrWhiteSpace(browserName) && browserName.ToLower() == Browsers.Firefox)
                    browser = BrowserTypeEnum.Firefox;
            }

            string username = string.Empty;

            using (var scrapper = new MeshimerScrapper(browser))
            {
                username = scrapper.GetUserNameFromMeshimerPageAndHandle(UsernameMismatchHandler);
                System.Console.Clear();
                System.Console.WriteLine(string.Format(Constants.UserNameFromMeshimerPage, username));
                Thread.Sleep(1000);
            }
        }

        static void UsernameMismatchHandler()
        {
            // do other stuff
        }
    }
}
