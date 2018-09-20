using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Service.RemoteInstaller.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            // ServiceController ctrl = new ServiceController("ServName", "max-pc");

            using (var scmHandle = NativeMethods.OpenSCManager("name", null, NativeMethods.SCM_ACCESS.SC_MANAGER_CREATE_SERVICE))
            {
                if (scmHandle.IsInvalid)
                {
                    System.Console.WriteLine("Its invalid, please press enter to exit!!!");
                    System.Console.Read();
                    return;
                }
                using (var serviceHandle = NativeMethods.CreateService(scmHandle,
                    "ServName",
                    "display name",
                    NativeMethods.SERVICE_ACCESS.SERVICE_ALL_ACCESS,
                    NativeMethods.SERVICE_TYPES.SERVICE_WIN32_OWN_PROCESS,
                    NativeMethods.SERVICE_START_TYPES.SERVICE_AUTO_START,
                    NativeMethods.SERVICE_ERROR_CONTROL.SERVICE_ERROR_NORMAL,
                    "service name",/*@"C:\Users\narek\Source\Repos\tools\RemoteServiceTool\Meshimer.Scrapper.Service\bin\Debug\Meshimer.Scrapper.Service.exe",*/
                    null,
                    IntPtr.Zero,
                    null,
                    null,
                    null))
                {
                    if (serviceHandle.IsInvalid)
                    {
                        System.Console.WriteLine("Its invalid, please press enter to exit!!!");
                        System.Console.Read();
                        return;
                    }
                }
            }
        }
    }
}
