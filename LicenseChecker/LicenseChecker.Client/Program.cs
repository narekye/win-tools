using LicenseChecker.Service;
using System;

namespace LicenseChecker.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new LicenseCheckerService();
            var checkedComputers = service.RunWindowsLicenseCheck();

            foreach (var computer in checkedComputers)
            {
                Console.WriteLine($"{computer.Key} | {computer.Value}");
            }

            Console.Read();
        }
    }
}
