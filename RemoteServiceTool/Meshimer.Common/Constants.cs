using System;

namespace Meshimer.Common
{
    public class Constants
    {
        public static string LogFolderLocation { get => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); }
        public const string LogFolderName = "Meshimer service logs";

        public const string ServiceName = "Meshimer.Scrapper";
        public const string ServiceDescription = "";
        public const string ServiceDisplayName = "Meshimer Scrapper Service";

        public const string wb_BlockDetails = "wb_BlockDetails";
        public const string UserNameFromMeshimerPage = "Username from meshimer page < {0} >";

        public const string AppStarted = "App started";
        public const string CurrentBrowser = "Current browser {0}";
        public const string BrowserOpened = "Browser opened !!!";
        public const string MeshimerServiceStartWithArgs = "Meshimer service started w/ args {0}";
        public const string MeshimerServiceStopped = "Meshimer service stopped !!!";
    }

    public class Tags
    {
        public const string Browser = "-browser";
        public const string Interval = "-interval";
    }

    public class Browsers
    {
        public const string Chrome = "chrome";
        public const string Firefox = "firefox";
    }
}
