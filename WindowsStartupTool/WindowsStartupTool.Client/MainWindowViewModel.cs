using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using WindowsStartupTool.Lib;

namespace WindowsStartupTool.Client
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private RegistryLookupSourceEnum _lookupSource;
        private bool _startRemoteRegistryServiceIfNeeded;
        private string _machineName;
        private RegistryEditor _registryEditor;
        private string _registryKey;
        private string _registryValue;
        private ObservableCollection<KeyValuePair<string, string>> _startupApps;

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

        public RelayCommand ShowStartupAppsCommand { get; }
        public RelayCommand SetRegistryCommand { get; }
        public RelayCommand RemoveAppFromStartup { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel()
        {
            MachineName = Environment.MachineName;
            ShowStartupAppsCommand = new RelayCommand(ShowStartupAppsExecute);
            SetRegistryCommand = new RelayCommand(SetRegistryExecute);
            LookupSource = RegistryLookupSourceEnum.Machine;
            RemoveAppFromStartup = new RelayCommand(RemoveAppFromStartupExecute);
        }

        void Notify([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        void ShowStartupAppsExecute(object param)
        {
            using (_registryEditor = new RegistryEditor(MachineName, LookupSource, startServiceIfNeeded: StartRemoteRegistryServiceIfNeeded))
            {
                try
                {
                    var result = _registryEditor.GetAllStartupAppsFromRegistry();
                    var builder = new StringBuilder();

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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            ShowStartupAppsExecute(null);
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
                    Notify();
                }
            }
        }
    }
}
