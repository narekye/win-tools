using System.ServiceProcess;

namespace Meshimer.Scrapper.Service
{
    public partial class MeshimerService : ServiceBase
    {
        public MeshimerService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
        }

        protected override void OnStop()
        {
        }
    }
}
