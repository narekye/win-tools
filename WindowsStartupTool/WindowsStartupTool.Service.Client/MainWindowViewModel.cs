using System.ComponentModel;
using System.Runtime.CompilerServices;
using WindowsStartupTool.Common.Constants;
using WindowsStartupTool.Lib;

namespace WindowsStartupTool.Service.Client
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Fields
        private string _serviceStartParams;
        private string _exeLocation;
        private string _email;
        private int _intervalInDays;
        private int _pingInterval;
        private readonly LocalServiceHelper _localServiceHelper;
        private static string _template = $"{Tags.Interval} " + "{0} " + $"{Tags.PingInterval} " + "{1} " + $"{Tags.Email} " + "{2}";

        #endregion

        #region Commands
        public RelayCommand InstallServiceCommand { get; }
        public RelayCommand StartServiceCommand { get; }
        public RelayCommand UpdateParamsCommand { get; }
        #endregion

        #region Constructors
        public MainWindowViewModel()
        {
            _localServiceHelper = new LocalServiceHelper();
            StartServiceCommand = new RelayCommand(StartServiceExecute, CanStartService);
            InstallServiceCommand = new RelayCommand(InstallServiceExecute, CanInstallService);
            UpdateParamsCommand = new RelayCommand(UpdateParamsExecute, CanUpdateParams);
        }

        #endregion

        #region Execute

        void StartServiceExecute(object param)
        {

        }

        bool CanStartService(object param)
        {
            return !string.IsNullOrWhiteSpace(ServiceStartParams);
        }

        void UpdateParamsExecute(object param)
        {
            ServiceStartParams = string.Format(_template, IntervalInDays, PingInterval, Email);
        }

        void InstallServiceExecute(object param)
        {
            _localServiceHelper.InstallService(ExeLocation, new string[] { });
        }

        bool CanInstallService(object param)
        {
            return !string.IsNullOrWhiteSpace(ExeLocation);
        }

        bool CanUpdateParams(object param)
        {
            return !string.IsNullOrWhiteSpace(Email) && Email.Contains("@");
        }
        #endregion

        #region Properties

        public int PingInterval
        {
            get { return _pingInterval; }
            set
            {
                if (_pingInterval != value)
                {
                    _pingInterval = value;
                    // AddPingIntervalCommand.NotifyCanExecuteChanged();
                    Notify();
                }
            }
        }

        public string Email
        {
            get { return _email; }
            set
            {
                if (_email != value)
                {
                    _email = value;
                    UpdateParamsCommand.NotifyCanExecuteChanged();
                    Notify();
                }
            }
        }

        public int IntervalInDays
        {
            get { return _intervalInDays; }
            set
            {
                if (_intervalInDays != value)
                {
                    _intervalInDays = value;
                    //AddDayCommand.NotifyCanExecuteChanged();
                    Notify();
                }
            }
        }

        public string ServiceStartParams
        {
            get { return _serviceStartParams; }
            set
            {
                if (_serviceStartParams != value)
                {
                    _serviceStartParams = value;
                    Notify();
                }
            }
        }

        public string ExeLocation
        {
            get { return _exeLocation; }
            set
            {
                if (_exeLocation != value)
                {
                    _exeLocation = value;
                    InstallServiceCommand.NotifyCanExecuteChanged();
                    Notify();
                }
            }
        }
        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        void Notify([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
