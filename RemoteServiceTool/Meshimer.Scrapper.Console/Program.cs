using Meshimer.Common;
using Meshimer.Common.Logger;
using Meshimer.Scrapper.BLL;
using System;
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

            Logger.Instance.LogMessage(Constants.AppStarted);

            if (args != null && args.Any())
            {
                var browserName = args[1].Trim();
                if (!string.IsNullOrWhiteSpace(browserName) && browserName.ToLower() == Browsers.Firefox)
                    browser = BrowserTypeEnum.Firefox;
            }

            Logger.Instance.LogMessage(string.Format(Constants.CurrentBrowser, Enum.GetName(typeof(BrowserTypeEnum), browser)));

            string username = string.Empty;

            using (var scrapper = new MeshimerScrapper(browser))
            {
                Thread.Sleep(1500);
                Logger.Instance.LogMessage(Constants.BrowserOpened);
                username = scrapper.GetUserNameFromMeshimerPageAndHandle(UsernameMismatchHandler);
                var logMessage = string.Format(Constants.UserNameFromMeshimerPage, username);

                Logger.Instance.LogMessage(logMessage);

                System.Console.Clear();
                System.Console.WriteLine(logMessage);
            }
        }

        static void UsernameMismatchHandler()
        {
            // do other stuff
        }
    }
}
