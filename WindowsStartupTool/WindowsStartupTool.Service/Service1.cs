using Service.AppRunner;
using System.ServiceProcess;

namespace WindowsStartupTool.Service
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            string path = @"C:\Users\narek\Source\Repos\tools\WindowsStartupTool\WindowsStartupTool.Console\bin\Debug\WindowsStartupTool.Console.exe";

            AppLoader.StartProcessAndBypassUAC(path, out AppLoader.PROCESS_INFORMATION prc);
        }

        protected override void OnStop()
        {
        }
    }
}
