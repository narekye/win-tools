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
            var configuration = new FileServiceConfiguration();


            var f = new FileService(configuration, "max-pc");
            //foreach (var t in f.GetLargeFilesFromComputers())
            //{
            
            //}

            var user = "oleg";
            foreach (var t in f.GetFilesFromComputersByUser(user))
            {
                  System.Console.WriteLine($"{t.Name} | {t.Size} | {t.Extension} ");
            }

        }
    }
}
