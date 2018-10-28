using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using WindowsStartupTool.Lib;
using System.Linq;
using WindowsStartupTool.Client.AppsWindow;

namespace WindowsStartupTool.Client
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Fields

        const string CheckAll = "Check All";
        const string UnCheckAll = "UnCheck All";

        private readonly BackgroundWorker _backgroundWorker;

        ProcessController.Lib.Library _lib = new ProcessController.Lib.Library();
        private RegistryLookupSourceEnum _lookupSource;
        private SkippableSourceEnum _skippableSource;
        private ComputerSourceTypeEnum _computerSourceType;
        private FileManager _fileManager;
        private bool _isPingEnabled;
        private string _pingTooltip;

        private bool _startRemoteRegistryServiceIfNeeded;
        private bool _skipDefaults;
        private bool _filterByPc;
        private int _pingTimeout;
        private string _machineName;
        private string _comboContent;
        private RegistryEditor _registryEditor;
        private string _registryKey;
        private string _registryValue;
        private ObservableCollection<KeyValuePair<string, string>> _startupApps;
        private ObservableCollection<SelectableComputer> _domainComputers;
        private string _selectAllBtnText;
        private ObservableCollection<string> _allComputers;
        private bool _thisMachine;
        #endregion

        #region Commands

        public RelayCommand ShowStartupAppsCommand { get; }
        public RelayCommand SetRegistryCommand { get; }
        public RelayCommand RemoveAppFromStartup { get; }
        public RelayCommand ShowStartupAppsFromSelectedComputers { get; }
        public RelayCommand SelectAllComputers { get; }
        public RelayCommand ViewSkippableKeys { get; }
        public RelayCommand DefaultTimeoutCommand { get; }
        public RelayCommand PingComputersCommand { get; }
        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        void Notify([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Constructors

        public MainWindowViewModel()
        {
            ShowStartupAppsCommand = new RelayCommand(ShowStartupAppsExecute);
            ViewSkippableKeys = new RelayCommand(ViewSkippableKeysExecute);
            SetRegistryCommand = new RelayCommand(SetRegistryExecute, CanExecuteSetRegistry);
            RemoveAppFromStartup = new RelayCommand(RemoveAppFromStartupExecute);
            ShowStartupAppsFromSelectedComputers = new RelayCommand(ShowStartupAppsFromSelectedComputersExecute, CanExecuteShowStartupApps);
            SelectAllComputers = new RelayCommand(SelectAllComputersExecute);
            PingComputersCommand = new RelayCommand(PingComputersExecute);
            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.DoWork += PingComputers;
            _backgroundWorker.RunWorkerCompleted += RunWorkerCompleted;

            SkipDefaults = true;
            LookupSource = RegistryLookupSourceEnum.Machine;
            StartRemoteRegistryServiceIfNeeded = true;
            _fileManager = new FileManager();
            ComputerSourceType = ComputerSourceTypeEnum.File;
            FilterByPc = true;
            SelectAllBtnText = CheckAll;
            SkippableSource = SkippableSourceEnum.Default;

            SetDefaultTimeoutExecute(null);
            IsPingEnabled = true;
            PingTooltip = "Press ping to find offline computers.";
        }

        #endregion

        #region Predicates

        bool CanExecuteShowStartupApps(object param)
        {
            return DomainComputers?.Any(x => x.IsSelected) ?? false;
        }

        bool CanExecuteSetRegistry(object param)
        {
            return !string.IsNullOrWhiteSpace(RegistryKey) && !string.IsNullOrEmpty(RegistryValue) && !string.IsNullOrEmpty(MachineName);
        }

        #endregion

        #region Execute

        void ShowStartupAppsFromSelectedComputersExecute(object param)
        {
            var dataToShow = new List<NodeItem>();

            var computers = DomainComputers?.Where(x => x.IsSelected).ToList();

            foreach (var computer in computers)
            {
                // Add ping here also
                using (_registryEditor = new RegistryEditor(computer?.Name, LookupSource, StartRemoteRegistryServiceIfNeeded))
                {
                    Dictionary<string, string> apps = null;

                    try
                    {
                        apps = _registryEditor.GetAllStartupAppsFromRegistry(SkippableSource);
                    }
                    catch
                    {
                    }

                    var data = apps?.Select(x => new KeyValuePair<string, string>(x.Key, x.Value)).ToList();

                    string name = string.Empty;

                    if (data == null)
                        name = $"{computer?.Name} / (Not available)";
                    else if (!data.Any())
                        name = $"{computer?.Name} / NO EXTRA APPS";
                    else
                        name = $"{computer?.Name}";

                    dataToShow.Add(new NodeItem
                    {
                        ComputerName = name,
                        Data = new ObservableCollection<KeyValuePair<string, string>>(data ?? new List<KeyValuePair<string, string>>())
                    });
                }
            }
            var window = new StartupAppsWindow();
            var viewModel = new StartupAppsWindowViewModel(dataToShow);
            window.DataContext = viewModel;
            window.Show();

        }

        void ShowStartupAppsExecute(object param)
        {
            using (_registryEditor = new RegistryEditor(MachineName, LookupSource, startServiceIfNeeded: StartRemoteRegistryServiceIfNeeded))
            {
                try
                {
                    var result = _registryEditor.GetAllStartupAppsFromRegistry(SkippableSource);
                    StartupApps = new ObservableCollection<KeyValuePair<string, string>>(result);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        void RemoveAppFromStartupExecute(object param)
        {
            string registryKey = param as string;
            if (!string.IsNullOrEmpty(registryKey))
            {
                _registryEditor.RemoveStartupAppByKey(registryKey);
            }
            ShowStartupAppsExecute(null);
        }

        void SetRegistryExecute(object param)
        {
            try
            {
                using (_registryEditor = new RegistryEditor(MachineName, LookupSource, StartRemoteRegistryServiceIfNeeded))
                {
                    _registryEditor.SetStartupAppToRegistry(RegistryKey, RegistryValue);
                }
                MessageBox.Show("Success !", "", MessageBoxButton.OK, MessageBoxImage.Information);
                RegistryKey = string.Empty;
                RegistryValue = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            ShowStartupAppsExecute(null);
        }

        void SelectAllComputersExecute(object param)
        {
            bool IsAnyChecked = SelectAllBtnText == CheckAll;

            if (IsAnyChecked)
                SelectAllBtnText = UnCheckAll;
            else
                SelectAllBtnText = CheckAll;

            foreach (var item in DomainComputers)
            {
                if (item.IsEnabled)
                    item.IsSelected = IsAnyChecked;
            }
        }

        void UpdateComputersList()
        {
            if (FilterByPc)
            {
                DomainComputers = new ObservableCollection<SelectableComputer>(_allComputers?.Where(x => x.EndsWith("pc", StringComparison.InvariantCultureIgnoreCase)).OrderBy(x => x)
                .Select(x => { var item = new SelectableComputer { Name = x, IsSelected = false, IsEnabled = true }; item.PropertyChanged += UpdateString; return item; }));
            }
            else
            {
                DomainComputers = new ObservableCollection<SelectableComputer>(_allComputers?.OrderBy(x => x).Select(x => { var item = new SelectableComputer { Name = x, IsSelected = false, IsEnabled = true }; item.PropertyChanged += UpdateString; return item; }));
            }
        }

        void LoadCorrespondingComputers(ComputerSourceTypeEnum value)
        {
            switch (value)
            {
                case ComputerSourceTypeEnum.Network:
                    try
                    {
                        _allComputers = new ObservableCollection<string>(_lib.GetCurrentDomainComputers().OrderBy(x => x));
                    }
                    catch (Exception)
                    {
                        _allComputers = new ObservableCollection<string> { Environment.MachineName };
                    }
                    break;
                case ComputerSourceTypeEnum.File:
                    _allComputers = new ObservableCollection<string>(_fileManager.GetComputerNames());
                    break;
            }

            UpdateComputersList();
        }

        void PingComputersExecute(object param)
        {
            _backgroundWorker.RunWorkerAsync();
            IsPingEnabled = false;
            PingTooltip = "Ping is in progress, please wait...";
        }

        void PingComputers(object sender, DoWorkEventArgs e)
        {
            foreach (var computer in DomainComputers)
            {
                var pinged = _lib.Ping(computer.Name, PingTimeout); ;
                computer.IsEnabled = pinged;

                if (!pinged && computer.IsSelected)
                    computer.IsSelected = false;
            }
        }

        void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsPingEnabled = true;
            PingTooltip = "Press ping to find offline computers.";
        }

        void UpdateString(object o, PropertyChangedEventArgs e)
        {
            if (DomainComputers != null)
            {
                var selectedData = DomainComputers.Where(x => x.IsSelected);
                ComboContent = string.Join(", ", selectedData.Select(x => x.Name));
            }

            try
            {
                ShowStartupAppsFromSelectedComputers.NotifyCanExecuteChanged();
            }
            catch
            {
            }
        }

        void ViewSkippableKeysExecute(object param)
        {
            var window = new SkipKeysWindow.SkipKeysWindow();
            window.DataContext = new SkipKeysWindow.SkipKeysWindowViewModel();

            window.Show();
        }

        void SetDefaultTimeoutExecute(object param)
        {
            PingTimeout = 500;
        }

        #endregion

        #region Properties

        public string PingTooltip
        {
            get { return _pingTooltip; }
            set
            {
                if (_pingTooltip != value)
                {
                    _pingTooltip = value;
                    Notify();
                }
            }
        }

        public bool IsPingEnabled
        {
            get { return _isPingEnabled; }
            set
            {
                if (_isPingEnabled != value)
                {
                    _isPingEnabled = value;
                    Notify();
                }
            }
        }

        public ObservableCollection<string> AllComputers
        {
            get { return _allComputers; }
            set
            {
                if (_allComputers != value)
                {
                    _allComputers = value; Notify();
                }
            }
        }

        public bool ThisMachine
        {
            get { return _thisMachine; }
            set
            {
                if (_thisMachine != value)
                {
                    _thisMachine = value;
                    if (value)
                    {
                        MachineName = Environment.MachineName;
                        StartRemoteRegistryServiceIfNeeded = false;
                    }
                    else
                        MachineName = _allComputers?.FirstOrDefault();
                }
            }
        }

        public ObservableCollection<SelectableComputer> DomainComputers
        {
            get { return _domainComputers; }
            set
            {
                if (_domainComputers != value)
                {
                    _domainComputers = value;
                    ShowStartupAppsFromSelectedComputers.NotifyCanExecuteChanged();
                    Notify();
                }
            }
        }

        public SkippableSourceEnum SkippableSource
        {
            get { return _skippableSource; }
            set
            {
                if (_skippableSource != value)
                {
                    _skippableSource = value;
                    ViewSkippableKeys?.NotifyCanExecuteChanged();
                    Notify();
                }
            }
        }

        public bool FilterByPc
        {
            get { return _filterByPc; }
            set
            {
                if (_filterByPc != value)
                {
                    _filterByPc = value;
                    UpdateComputersList();
                    Notify();
                }
            }
        }

        public int PingTimeout
        {
            get { return _pingTimeout; }
            set
            {
                if (_pingTimeout != value)
                {
                    _pingTimeout = value;
                    Notify();
                }
            }
        }

        public bool SkipDefaults
        {
            get { return _skipDefaults; }
            set
            {
                if (_skipDefaults != value)
                {
                    _skipDefaults = value;
                    Notify();
                }
            }
        }

        public ObservableCollection<KeyValuePair<string, string>> StartupApps
        {
            get { return _startupApps; }
            set
            {
                if (_startupApps != value)
                {
                    _startupApps = value;
                    Notify();
                }
            }
        }

        public RegistryLookupSourceEnum LookupSource
        {
            get { return _lookupSource; }
            set
            {
                if (_lookupSource != value)
                {
                    _lookupSource = value;
                    Notify();
                }
            }
        }

        public string RegistryKey
        {
            get { return _registryKey; }
            set
            {
                if (_registryKey != value)
                {
                    _registryKey = value;
                    SetRegistryCommand.NotifyCanExecuteChanged();
                    Notify();
                }
            }
        }

        public string ComboContent
        {
            get { return _comboContent; }
            set
            {
                if (_comboContent != value)
                {
                    _comboContent = value;
                    Notify();
                }
            }
        }

        public string RegistryValue
        {
            get { return _registryValue; }
            set
            {
                if (_registryValue != value)
                {
                    _registryValue = value;
                    SetRegistryCommand.NotifyCanExecuteChanged();
                    Notify();
                }
            }
        }

        public bool StartRemoteRegistryServiceIfNeeded
        {
            get { return _startRemoteRegistryServiceIfNeeded; }
            set
            {
                if (_startRemoteRegistryServiceIfNeeded != value)
                {
                    _startRemoteRegistryServiceIfNeeded = value;
                    Notify();
                }
            }
        }

        public string MachineName
        {
            get { return _machineName; }
            set
            {
                if (_machineName != value)
                {
                    _machineName = value;
                    SetRegistryCommand?.NotifyCanExecuteChanged();
                    Notify();
                }
            }
        }

        public string SelectAllBtnText
        {
            get { return _selectAllBtnText; }
            set
            {
                if (_selectAllBtnText != value)
                {
                    _selectAllBtnText = value;
                    Notify();
                }
            }
        }

        public ComputerSourceTypeEnum ComputerSourceType
        {
            get { return _computerSourceType; }
            set
            {
                if (_computerSourceType != value)
                {
                    _computerSourceType = value;
                    LoadCorrespondingComputers(value);
                    Notify();
                }
            }
        }

        #endregion
    }
}
