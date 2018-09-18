using Meshimer.Common;
using Meshimer.Common.Logger;
using System;
using System.Linq;
using System.ServiceProcess;
using System.Timers;

namespace Meshimer.Scrapper.Service
{
    public partial class MeshimerService : ServiceBase
    {
        private int _interval;
        private string _browser;
        private Timer _timer;

        public MeshimerService()
        {
            InitializeComponent();
            _timer = new Timer();
        }

        protected override void OnStart(string[] args)
        {
            HandleInputArgs(args);
            Logger.Instance.LogMessage(string.Format(Constants.MeshimerServiceStartWithArgs, string.Join(" ", args)));
            _timer.Elapsed += OnElapsedTime;
            _timer.Interval = _interval == 0 ? TimeSpan.FromHours(1).TotalMilliseconds : _interval;
            _timer.Enabled = true;
        }

        protected override void OnStop()
        {
            Logger.Instance.LogMessage(Constants.MeshimerServiceStopped);
        }

        void HandleInputArgs(string[] args)
        {
            // Should be like this format
            // -browser chrome -interval 2000
            try
            {
                if (args != null && args.Any())
                {
                    if (args[0] == Tags.Browser)
                        _browser = args[1].ToLower();
                    if (args[2] == Tags.Interval)
                        _interval = int.Parse(args[3]);
                }
            }
            catch { }
        }

        void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            // App runner....
        }
    }
}
