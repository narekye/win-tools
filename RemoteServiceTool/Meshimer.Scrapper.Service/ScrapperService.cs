using Meshimer.Common;
using Meshimer.Common.Logger;
using System;
using System.IO;
using System.ServiceProcess;
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

        #region Public methods

        public void Start(string[] args)
        {
            OnStart(args);
        }

        public void Start()
        {
            OnStart(null);
        }

        #endregion

        #region Override

        protected override void OnStart(string[] args)
        {
            HandleArguments(args);
            _timer.Interval = _interval;
            _timer.Enabled = true;
        }

        protected override void OnStop()
        {
            Logger.Instance.LogMessage(Constants.MeshimerServiceStopped);
            _timer.Enabled = false;
            _timer.Stop();
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
                _browser = Browsers.Chrome;
                _interval = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                Logger.Instance.LogMessage(string.Format(Constants.MeshimerServiceStartedWithDefaultArgs, _browser, _interval));
            }
        }

        void TimerElapsed(object source, ElapsedEventArgs e)
        {
            if (!File.Exists(Constants.MeshimerScrapperConsoleExeLocation))
            {
                Logger.Instance.LogMessage(Constants.MeshimerExeFileNotFound);

                if (Environment.UserInteractive)
                    Console.WriteLine(Constants.MeshimerExeFileNotFound);

                return;
            }

            PROCESS_INFORMATION prc = new PROCESS_INFORMATION();
            StartProcessAndBypassUAC($@"{Constants.MeshimerScrapperConsoleExeLocation} {Tags.Browser} {_browser} {Tags.Interval} {_interval}", out prc);
            Logger.Instance.LogMessage(string.Format(Constants.ProcessStartedWithPID, prc.dwProcessId));
        }

        #endregion
    }
}
