using ProcessController.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Timers;
using WindowsStartupTool.Common.Constants;
using WindowsStartupTool.Lib;

namespace WindowsStartupTool.Service
{
    public partial class StartupService : ServiceBase
    {
        private Timer _timer;
        private string _email;
        private IEnumerable<string> _computers;

        private int _pingTimeout;

        public StartupService()
        {
            InitializeComponent();
            _timer = new Timer();
            _timer.Elapsed += Elapsed;
        }

        void Elapsed(object sender, ElapsedEventArgs e)
        {
            var lib = new Library();

            _computers = lib.GetCurrentDomainComputers();

            if (_computers == null)
                return;

            foreach (var computer in _computers)
            {
                bool isOnline = lib.Ping(computer, _pingTimeout);

                string line = $"PC name: {computer} | Status : {(isOnline ? "Online" : "Offline")}";

                if (!isOnline)
                    continue; // need to build here a report

                using (var regedit = new RegistryEditor(computer, startServiceIfNeeded: true)) // default value, service may not start
                {
                    var startupApps = regedit.GetAllStartupAppsFromRegistry(SkippableSourceEnum.None);

                    foreach (var application in startupApps)
                    {
                        if (regedit.DefaultStartupApps.Any(x => x == application.Key))
                        {
                            string val = $"Found default startup app {application.Key} | Path: {application.Value}";
                        }
                        else // means of found extra application
                        {
                            string val = $"Found an extra app {application.Key} | Path: {application.Value}";

                            try
                            {
                                regedit.RemoveStartupAppByKey(application.Key);
                                // append to text, application removed successfully.
                            }
                            catch (Exception ex)
                            {
                                // write error;
                            }
                        }
                    }
                }
            }
        }

        protected override void OnStart(string[] args)
        {
            HandleStartArgs(args);
            _timer.Enabled = true;
        }

        protected override void OnStop()
        {
        }

        void HandleStartArgs(string[] args)
        {
            _pingTimeout = 500;
            int interval = 7;

            if (args.Any(x => x == Tags.Interval)) // in days
            {
                var intervalString = args[1];
                if (int.TryParse(intervalString, out int pInterval))
                {
                    if (interval < pInterval)
                        interval = pInterval;
                }
            }

            if (args.Any(x => x == Tags.PingInterval))
            {
                int.TryParse(args[3], out int pingInterval);
                if (pingInterval > 0)
                    _pingTimeout = pingInterval;
            }

            if (args.Any(x => x == Tags.Email))
            {
                _email = args[5];
            }

            _timer.Interval = interval;
        }
    }
}
