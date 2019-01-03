using System;
using System.Configuration.Install;
using System.IO;
using System.ServiceProcess;

namespace WindowsStartupTool.Lib
{
    public class LocalServiceHelper
    {
        public LocalServiceHelper()
        {

        }

        public void InstallService(string exeFileName, string[] args)
        {
            if (!File.Exists(exeFileName))
                return;
            AssemblyInstaller installer = new AssemblyInstaller(exeFileName, args);
            installer.UseNewContext = true;
            installer.Install(null);
            installer.Commit(null);
            installer.Dispose();
        }

        public void UnInstallService(string exeFileName)
        {
            if (!File.Exists(exeFileName))
                return;

            AssemblyInstaller installer = new AssemblyInstaller(exeFileName, null);
            installer.UseNewContext = true;
            installer.Uninstall(null);
            installer.Dispose();
        }

        public bool StartService(string serviceName, string[] args)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
                throw new ArgumentException();

            using (ServiceController controller = new ServiceController(serviceName))
            {
                if (controller.Status == ServiceControllerStatus.Running)
                    throw new Exception("Service already started");

                controller.Start(args);
                return controller.Status == ServiceControllerStatus.Running;
            }
        }
    }
}
