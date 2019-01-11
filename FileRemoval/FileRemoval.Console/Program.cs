using FileRemoval.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileRemoval.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            // create file service configuration, now service will catch all files greather than 1 MB.
            var configuration = new FileServiceConfiguration(FileSizeEnum.MB);

            // build a folder path where to create log files.

            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Logs_for_fileService";

            // create logger instance, passing in folder path.
            // as second parameter passing LogBehaviorEnum.LogOnlyExceptions to log only exceptions, default value is LogBehaviorEnum.LogEverything
            var logger = new FileServiceLogger(folderPath, LogBehaviorEnum.LogOnlyExceptions);

            // assign logger to configuraion instance,
            configuration.ServiceLogger = logger;

            // create file service instance. service will query only max-pc
            var fileService = new FileService(configuration, "max-pc" /*, "cubicle23-pc", etc...*/);

            #region These calls will not delete any file...

            // retrive data by username
            //var data = fileService.GetLargeFilesFromComputers("oleg");

            //// The method above will query to all users in computers
            ////var data = fileService.GetLargeFilesFromComputers();

            //// just to displaying the data
            //foreach (var item in data)
            //{
            //    System.Console.WriteLine($"{item.ComputerName} {item.Name} {item.Note} | {item.Status}");
            //}

            #endregion

            #region These call will delete file if any...

            // without username
            // fileService.DeleteLargeFiles()

            // with username
            //var data1 = fileService.DeleteLargeFiles("narek");

            //foreach (var item in data1)
            //{
            //    // Currently I don't have any files in max-pc computer, this list is empty
            //}

            #endregion

            fileService.DeleteProfile("cubicle23-pc", "narek");
            

            System.Console.Read();
        }
    }
}
