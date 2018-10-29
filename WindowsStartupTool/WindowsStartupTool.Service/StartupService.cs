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

        private IEnumerable<string> _computers;
        private ComputerSourceTypeEnum _computerSource;
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

            switch (_computerSource)
            {
                case ComputerSourceTypeEnum.Network:
                    _computers = lib.GetCurrentDomainComputers();
                    break;
                case ComputerSourceTypeEnum.File:
                    // var fileManager = 
                    // Some problems, need to discuss
                    break;
            }

            if (_computers == null)
                return;

            foreach (var computer in _computers)
            {
                bool isOnline = lib.Ping(computer, _pingTimeout);

                string line = $"PC name: {computer} | Status : {( isOnline ? "Online" : "Offline")}";

            }
        }

        protected override void OnStart(string[] args)
        {
            HandleStartArgs(args);
            _timer.Enabled = true;
            // _timer.Interval = TimeSpan.FromDays(7).TotalMilliseconds; // one week
        }

        protected override void OnStop()
        {
        }

        void HandleStartArgs(string[] args)
        {
            _computerSource = ComputerSourceTypeEnum.Network;
            _pingTimeout = 500;
            double interval = TimeSpan.FromDays(7).TotalMilliseconds;

            if (args.Any(x => x == Tags.Source))
            {
                var source = args[1];
                if (int.TryParse(source, out int sourceId))
                {
                    _computerSource = (ComputerSourceTypeEnum)sourceId;
                }
            }

            if (args.Any(x => x == Tags.Interval)) // in days
            {
                var intervalString = args[3];
                if (double.TryParse(intervalString, out double pInterval))
                {
                    if (interval < pInterval)
                        interval = pInterval;
                }
            }

            if (args.Any(x => x == Tags.PingInterval))
            {
                int.TryParse(args[5], out int pingInterval);
                if (pingInterval > 0)
                    _pingTimeout = pingInterval;
            }

            _timer.Interval = interval;
        }
    }
}
