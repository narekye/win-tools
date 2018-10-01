using Meshimer.Scrapper.Service;
using System;
using System.Threading;

namespace Meshimer.Scrapper.SerivceInstaller
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new ScrapperService();
            Console.WriteLine("/r run, /r -args run with args, /i install, /u uninstall");
            var command = Console.ReadLine();
            while (!string.IsNullOrWhiteSpace(command))
            {
                switch (command)
                {
                    case "/r":
                        Console.WriteLine("Run service...");
                        Thread.Sleep(1500);
                        service.Start();
                        Console.WriteLine("Success...");
                        break;
                    case "/i":
                        break;
                    case "/u":
                        break;
                    default:
                        break;
                }

                command = Console.ReadLine();
            }

            Console.WriteLine("Please press enter to exit !!!");
            Console.ReadLine();
        }
    }
}
