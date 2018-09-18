using System;
using System.IO;

namespace Meshimer.Common.Logger
{
    public class Logger
    {
        private static Logger _logger;
        private string _directoryPath;
        private string _fileName;

        private object _locker = new object();

        public static Logger Instance { get { return _logger ?? (_logger = new Logger()); } }

        string FullFilePath { get => Path.Combine(_directoryPath, _fileName); }

        private Logger()
        {
            _directoryPath = Path.Combine(Constants.LogFolderLocation, Constants.LogFolderName);
            if (!Directory.Exists(_directoryPath))
                Directory.CreateDirectory(_directoryPath);
            _fileName = CreateOrSkipFile();
        }

        public void LogMessage(string message)
        {
            var currentTime = DateTime.Now.ToString();
            var appendText = $"{currentTime} : {message}";

            lock (_locker)
                File.AppendAllText(FullFilePath, appendText);
        }

        string CreateOrSkipFile()
        {
            string commonName = $"Service_log_{DateTime.Now.Date.ToShortDateString().Replace('/', '-')}.txt";

            if (!File.Exists(Path.Combine(_directoryPath, commonName)))
                using (File.Create(Path.Combine(_directoryPath, commonName))) { }

            return commonName;
        }
    }
}
