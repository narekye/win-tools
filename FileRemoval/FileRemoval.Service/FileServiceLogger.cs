using System;
using System.IO;

namespace FileRemoval.Service
{
    public class FileServiceLogger
    {
        private string _directory;
        private const string Template = "{0} | {1} | {2} | {3} | {4} | {5} \r\n";

        public LogBehaviorEnum LogBehavior { get; private set; }

        private string FileName
        {
            get
            {
                var currentDateString = DateTime.Now.ToShortDateString().Replace('/', '_');
                return currentDateString;
            }
        }

        public FileServiceLogger(string directory, LogBehaviorEnum logBehavior = LogBehaviorEnum.LogEverything)
        {
            _directory = directory;
            LogBehavior = logBehavior;
            CreateOrSkipDirectory();
        }

        private void CreateOrSkipDirectory()
        {
            if (!Directory.Exists(_directory))
            {
                Directory.CreateDirectory(_directory);
            }
        }

        public void Info(FileModel file)
        {
            var fullPath = $"{_directory}/{FileName}.txt";
            var message = string.Format(Template, "INFO", file.ComputerName, file.Username, file.Name, file.ReadableSize, file.CreatedDate.ToShortDateString());
            File.AppendAllText(fullPath, message);
        }

        public void Error(FileModel file)
        {
            var fullPath = $"{_directory}\\{FileName}.txt";
            var message = string.Format("ERROR | {1} | {2} | {3} \r\n", "ERROR", file.ComputerName, file.Username, file.Note);
            //var message = string.Format(Template, "Error", file.ComputerName, file.Username, file.Name, file.ReadableSize, file.CreatedDate.ToShortDateString());
            File.AppendAllText(fullPath, message);
        }
    }
}
