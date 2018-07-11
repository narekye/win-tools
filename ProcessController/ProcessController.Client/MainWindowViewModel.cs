using ProcessController.Lib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace ProcessController.Client
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Constructors

        public MainWindowViewModel()
        {
            LoadControl();

            RunNewProcessCommand = new RelayCommand(RunProcessCommandExecute, o => !string.IsNullOrWhiteSpace(NewProcessName));
            GetLoggedInUsersCommand = new RelayCommand(GetLoggedInUsersExecute, o => SelectedMachine != null);
            TerminateCommand = new RelayCommand(TerminateCommandExecute);
            GetProcessesCommand = new RelayCommand(GetProcessesExecute, CanExecuteGetProcesses);
            CheckIfUserIsLoggedOn = new RelayCommand(CheckIfUserIsLoggedIn);
        }

        #endregion

        #region Event
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Notify
        protected void Notify([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Fields

        private ObservableCollection<string> _networks;
        private ObservableCollection<ProcessModel> _processes;
        private string _selectedNetwork;
        private string _availableSpaceOnDrive;
        private string _newProcessName;

        private string _username;
        private string _password;

        private bool _checkDomain;
        private string _checkUserName;

        private Library _library;

        #endregion

        #region Properties
        public string CheckUserName
        {
            get { return _checkUserName; }
            set
            {
                if (_checkUserName != value)
                {
                    _checkUserName = value;
                    Notify();
                }
            }
        }
        public string Username
        {
            get { return _username; }
            set
            {
                if (_username != value)
                {
                    _username = value;
                    GetProcessesCommand.NotifyCanExecuteChanged();
                    Notify();
                }
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    GetProcessesCommand.NotifyCanExecuteChanged();
                    Notify();
                }
            }
        }

        public string NewProcessName
        {
            get { return _newProcessName; }
            set
            {
                if (_newProcessName != value)
                {
                    _newProcessName = value;
                    RunNewProcessCommand.NotifyCanExecuteChanged();
                    Notify();
                }
            }
        }

        public ObservableCollection<ProcessModel> Processes
        {
            get { return _processes; }
            set
            {
                if (_processes != value)
                {
                    _processes = value;
                    Notify();
                }
            }
        }

        public string AvailableSpaceOnDrive
        {
            get { return _availableSpaceOnDrive; }
            set
            {
                if (_availableSpaceOnDrive != value)
                {
                    _availableSpaceOnDrive = value;
                    Notify();
                }
            }

        }

        public string SelectedMachine
        {
            get { return _selectedNetwork; }
            set
            {
                if (_selectedNetwork != value)
                {
                    _selectedNetwork = value;
                    LoadAvailableSpace();
                    Processes = new ObservableCollection<ProcessModel>();
                    GetLoggedInUsersCommand.NotifyCanExecuteChanged();
                    Notify();
                }
            }
        }

        public ObservableCollection<string> Networks
        {
            get { return _networks; }
            set
            {
                if (_networks != value)
                {
                    _networks = value;
                    Notify();
                }
            }
        }

        public bool CheckDomain
        {
            get { return _checkDomain; }
            set
            {
                if (_checkDomain != value)
                {
                    _checkDomain = value;
                    Notify();
                }
            }
        }

        #endregion

        #region Command

        public RelayCommand RunNewProcessCommand { get; }

        public RelayCommand TerminateCommand { get; }

        public RelayCommand GetProcessesCommand { get; }

        public RelayCommand GetLoggedInUsersCommand { get; }

        public RelayCommand CheckIfUserIsLoggedOn { get; }
        #endregion

        #region Execute

        void RunProcessCommandExecute(object param)
        {
            bool result = _library.StartNewProcess(SelectedMachine, NewProcessName);
            if (!result)
                MessageBox.Show("Err");
            else
                MessageBox.Show("Runned succesfully.");
        }

        void TerminateCommandExecute(object param)
        {
            MessageBox.Show(param.ToString());
            if (param is string processName)
            {
                try
                {
                    _library.TerminateProcess(SelectedMachine, processName.ToString(), Username, Password);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                GetProcessesExecute(null);
            }
        }

        void GetProcessesExecute(object param)
        {
            try
            {
                if (SelectedMachine == "localhost")
                {
                    var prc = _library.GetProcesses(SelectedMachine);
                }
                var processes = _library.GetProcessesFromRemoteMachine(SelectedMachine, Username, Password, CheckDomain).OrderBy(x => x.Caption);
                Processes = new ObservableCollection<ProcessModel>(processes);
            }
            catch (System.Exception ex)
            {
                Processes = new ObservableCollection<ProcessModel>();
                MessageBox.Show(ex.Message);
            }
        }

        async void GetLoggedInUsersExecute(object param)
        {
            await Task.Run(() =>
            {
                try
                {
                    string msg = string.Empty;
                    var user = _library.WhoisLoggedIn(SelectedMachine);

                    msg = user;
                    MessageBox.Show(msg);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
        }

        void CheckIfUserIsLoggedIn(object param)
        {
            MessageBox.Show("Username to check:  " + CheckUserName);

            var machines = Networks.Where(x => x.StartsWith("cubicle", StringComparison.OrdinalIgnoreCase));

            MessageBox.Show("Cmputers to check:  " + string.Join(",", machines));

            var machineName = _library.CheckIfUserIsLoggedIntoMachine(machines, CheckUserName);
            if (!string.IsNullOrEmpty(machineName)) MessageBox.Show($"Found {machineName}");

        }

        #endregion

        #region Helpers

        bool CanExecuteGetProcesses(object param)
        {
            return true;
            // return Username != null && Password != null && SelectedMachine != null;
        }

        void LoadControl()
        {
            _library = new Library();
            try
            {
                var data = new List<string>(_library.GetCurrentDomainComputers());
                data.Sort();
                Networks = new ObservableCollection<string>(data);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        async void LoadAvailableSpace()
        {
            await Task.Run(() =>
            {
                if (SelectedMachine != null)
                    AvailableSpaceOnDrive = _library.GetDiskAvailableSpace(SelectedMachine);
            });
        }

        #endregion
    }
}
