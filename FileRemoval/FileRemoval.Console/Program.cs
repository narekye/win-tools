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
            var configuration = new FileServiceConfiguration(FileSizeEnum.MB);
            
            var fService = new FileService(configuration);

            var data = fService.GetLargeFilesFromComputers("oleg");

            foreach(var item in data)
            {
                System.Console.WriteLine($"{item.ComputerName} {item.Name} {item.Note} | {item.Status}");
            }
            
            System.Console.Read();
        }
    }
}
