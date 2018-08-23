using System;
using System.Collections.Generic;
using System.Linq;
//using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using WindowsStartupTool.Lib;

namespace WindowsStartupTool.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            string machine = "cubicle23-pc";

            using (var editor = new RegistryEditor(RegistryLookupSourceEnum.User, machine, true))
            {
                var apps = editor.GetStartupAppsFromRegistry();

                foreach (var app in apps)
                    System.Console.WriteLine($"{app.Key} - {app.Value}");
            }

            System.Console.ReadKey();
        }
    }
}
