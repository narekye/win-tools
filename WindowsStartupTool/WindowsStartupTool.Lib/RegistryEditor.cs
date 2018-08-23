using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;

namespace WindowsStartupTool.Lib
{
    public class RegistryEditor : IDisposable
    {
        private readonly string _machine;
        private readonly bool _startServiceIfNeeded;
        private readonly RegistryLookupSourceEnum _source;
        private RegistryKey _registry;
        const string StartupSubKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";

        public RegistryEditor(string machine, RegistryLookupSourceEnum source = RegistryLookupSourceEnum.Machine, bool startServiceIfNeeded = false)
        {
            _source = source;
            _startServiceIfNeeded = startServiceIfNeeded;
            _machine = machine;
            if (string.IsNullOrEmpty(machine))
                _machine = Environment.MachineName;
        }

        /// <summary>
        /// Returns the list of startup programs on remote machine, with executable file path's
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetStartupAppsFromRegistry()
        {
            InitializeRegistry(StartupSubKey);

            Dictionary<string, string> result = new Dictionary<string, string>();

            if (_registry != null)
            {

                foreach (var app in _registry.GetValueNames().AsEnumerable())
                {
                    var value = _registry.GetValue(app);
                    result.Add(app, value.ToString());
                }
            }
            return result;
        }

        /// <summary>
        /// If application with the same key is exists function will replace the value
        /// </summary>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <param name="executeablePath"></param>
        /// <param name="machine"></param>
        public void SetStartupAppToRegistry(string key, string executeablePath)
        {
            SetRegistry(StartupSubKey, key, executeablePath);
        }

        /// <summary>
        /// Initializes RegistryEditor to passed in registry key value
        /// </summary>
        /// <param name="subKey"></param>
        public void InitializeRegistry(string subKey = "")
        {
            if (string.IsNullOrWhiteSpace(_machine) || _machine == Environment.MachineName)
            {
                switch (_source)
                {
                    case RegistryLookupSourceEnum.Machine:
                        _registry = Registry.LocalMachine.OpenSubKey(subKey);
                        break;
                    default: // User
                        _registry = Registry.CurrentUser.OpenSubKey(subKey);
                        break;
                }
            }
            else
            {
                try
                {
                    if (_startServiceIfNeeded)
                    {
                        ServiceController serviceController = new ServiceController("Remote Registry", _machine);
                        if (serviceController.Status != ServiceControllerStatus.Running)
                            serviceController.Start();
                    }
                    switch (_source)
                    {
                        case RegistryLookupSourceEnum.Machine:
                            _registry = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, _machine).OpenSubKey(subKey, RegistryKeyPermissionCheck.ReadWriteSubTree);
                            break;
                        default:
                            _registry = RegistryKey.OpenRemoteBaseKey(RegistryHive.CurrentUser, _machine).OpenSubKey(subKey, RegistryKeyPermissionCheck.ReadWriteSubTree);
                            break;
                    }
                }
                catch (System.IO.IOException)
                {
                    var exceptionMessage = $"On <{_machine}> computer, < Remote Registry > service is not running.";
                    throw new Exception(exceptionMessage);
                }
            }
        }

        /// <summary>
        /// Sets the registry with specified sub key
        /// </summary>
        /// <param name="subKey"></param>
        /// <param name="registryKey"></param>
        /// <param name="registryValue"></param>
        public void SetRegistry(string subKey, string registryKey, string registryValue)
        {
            InitializeRegistry(subKey);

            if (_registry != null)
            {
                _registry.SetValue(registryKey, registryValue, RegistryValueKind.String);
            }
        }

        /// <summary>
        /// IDisposable support
        /// </summary>
        public void Dispose()
        {

            if (_registry != null)
                _registry.Close();
            GC.SuppressFinalize(this);
        }
    }
}
