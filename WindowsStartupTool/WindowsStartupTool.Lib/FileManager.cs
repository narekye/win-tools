using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WindowsStartupTool.Lib
{
    public class FileManager
    {
        const string _fileName = "SkipKeys.txt";

        /// <summary>
        /// Saves passed values to SkipFiles.txt files
        /// </summary>
        /// <param name="values"></param>
        public void SaveToFile(IEnumerable<string> values)
        {
            File.WriteAllText(_fileName, string.Join(",", values));
        }

        /// <summary>
        /// Get's the data from SkipKeys.txt file (See executable folder)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetFileContent()
        {
            if (!File.Exists(_fileName))
                using (File.Create(_fileName)) { }

            return File.ReadAllText(_fileName).Split(',').Select(x => x);
        }

        /// <summary>
        /// Exports data
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="data"></param>
        /// <param name="exportFileType"></param>
        public void ExportToFile(string folderPath, IEnumerable<NodeItem> data, ExportFileTypeEnum exportFileType = ExportFileTypeEnum.Csv)
        {
            switch (exportFileType)
            {
                case ExportFileTypeEnum.Csv:
                    ExportToCsv(folderPath, data);
                    break;
                case ExportFileTypeEnum.Json:
                    ExportToJson(folderPath, data);
                    break;
            }
        }

        /// <summary>
        /// Exports passed content to CSV format.
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="data"></param>
        void ExportToCsv(string folderPath, IEnumerable<NodeItem> data)
        {
            var fileName = $"{DateTime.Now.ToShortDateString().Replace('/', '-').ToString()}_startupApps.csv";
            using (var file = File.Create(Path.Combine(folderPath, fileName)))
            {
                var builder = new StringBuilder("computer_name,key,value");

                foreach (var computer in data)
                {
                    var innerBuilder = new StringBuilder();
                    if (computer.Data != null)
                    {
                        foreach (var startupApp in computer.Data)
                        {
                            innerBuilder.Append($"{computer.ComputerName},{startupApp.Key},{startupApp.Value}");
                            innerBuilder.Append(Environment.NewLine);
                        }
                    }
                    builder.Append(Environment.NewLine);
                    builder.Append(innerBuilder.ToString());
                }

                var result = Encoding.UTF8.GetBytes(builder.ToString());
                file.Write(result, 0, result.Length);
            }
        }

        /// <summary>
        /// Exports passed content to JSON format.
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="data"></param>
        void ExportToJson(string folderPath, IEnumerable<NodeItem> data)
        {
            var fileName = $"{DateTime.Now.ToShortDateString().Replace('/', '-').ToString()}_startupApps.json";
            using (var file = File.Create(Path.Combine(folderPath, fileName)))
            {
                var json = JsonConvert.SerializeObject(data);
                var contentToWrite = Encoding.UTF8.GetBytes(json);
                file.Write(contentToWrite, 0, contentToWrite.Length);
            }
        }
    }
}
