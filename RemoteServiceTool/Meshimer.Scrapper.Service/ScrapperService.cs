using Meshimer.Common.Logger;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Meshimer.Scrapper.Service
{
    public partial class ScrapperService : ServiceBase
    {
        public ScrapperService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Logger.Instance.LogMessage("Start");
        }

        protected override void OnStop()
        {
            Logger.Instance.LogMessage("End");
        }
    }
}
