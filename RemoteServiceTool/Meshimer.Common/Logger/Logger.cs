namespace Meshimer.Common.Logger
{
    public class Logger
    {
        private static Logger _logger;

        public static Logger Instance { get { return _logger ?? (_logger = new Logger()); } }

        private Logger() { }

        public void LogMessage(string message) { }
    }
}
