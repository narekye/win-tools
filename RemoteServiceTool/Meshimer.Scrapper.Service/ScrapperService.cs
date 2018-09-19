using Meshimer.Common;
using Meshimer.Common.Logger;
using Service.AppRunner;
using System;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Timers;
using static Service.AppRunner.AppLoader;

namespace Meshimer.Scrapper.Service
{
    public partial class ScrapperService : ServiceBase
    {
        #region Fields

        private int _interval;
        private string _browser;
        private Timer _timer;
        private int _counter = 1;
        #endregion

        #region Constructor

        public ScrapperService()
        {
            _timer = new Timer();
            _timer.Elapsed += TimerElapsed;
            InitializeComponent();
        }

        #endregion

        #region Override

        protected override void OnStart(string[] args) // -browser chrome  -interval 5000
        {
            HandleArguments(args);
            _timer.Interval = _interval;
            _timer.Enabled = true;
            //             AppLoader.StartProcessAndBypassUAC("cmd.exe", out PROCESS_INFORMATION prc);
        }

        protected override void OnStop()
        {
            Logger.Instance.LogMessage(Constants.MeshimerServiceStopped);
        }

        #endregion

        #region Private methods

        void HandleArguments(string[] args)
        {
            if (args != null && args.Length == 4)
            {
                var browserTag = args[0];
                if (!string.IsNullOrWhiteSpace(browserTag) && browserTag == Tags.Browser)
                    _browser = args[1];
                var intervalTag = args[2];
                if (!string.IsNullOrWhiteSpace(intervalTag) && intervalTag == Tags.Interval)
                    _interval = int.Parse(args[3]);
                Logger.Instance.LogMessage(string.Format(Constants.MeshimerServiceStartWithArgs, string.Join(" ", args ?? new string[0])));
            }
            else
            {
                _browser = "chrome";
                _interval = (int)TimeSpan.FromMinutes(1).TotalMilliseconds; // (int)TimeSpan.FromHours(1).TotalMilliseconds; // change this
                Logger.Instance.LogMessage(string.Format(Constants.MeshimerServiceStartedWithDefaultArgs, _browser, _interval));
            }
        }

        async void TimerElapsed(object source, ElapsedEventArgs e)
        {
            PROCESS_INFORMATION prc = new PROCESS_INFORMATION();
            await Task.Run(() => StartProcessAndBypassUAC(@"C:\Program Files\Meshimer\Meshimer.Scrapper.Console.exe", out prc));
            Logger.Instance.LogMessage(string.Format(Constants.ProcessStartedWithPID, prc.dwProcessId));
        }

        #endregion
    }
}
