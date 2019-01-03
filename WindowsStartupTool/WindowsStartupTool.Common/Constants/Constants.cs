namespace WindowsStartupTool.Common.Constants
{
    public static class Constants
    {
        public const string ServiceName = "StartupReportService";
        public const string ServiceDescription = "Used to retrive all computers startup apps information from current domain";
        public const string ServiceDisplayName = "Startup Report Service";
    }

    public static class Tags
    {
        public const string Interval = "--interval";
        public const string Source = "--source";
        public const string PingInterval = "--pingInterval";
        public const string Email = "--email";
    }
}
