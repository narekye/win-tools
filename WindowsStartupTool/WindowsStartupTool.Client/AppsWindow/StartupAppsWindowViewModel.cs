using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Forms;
using WindowsStartupTool.Lib;

namespace WindowsStartupTool.Client.AppsWindow
{
    public class StartupAppsWindowViewModel : INotifyPropertyChanged
    {
        #region Fields

        private IEnumerable<NodeItem> _data;
        private ObservableCollection<NodeItem> _computersData;
        private RegistryEditor registryEditor;
        private ExportFileTypeEnum _fileType;
        private string _folderPath;
        private readonly FileManager _fileManager;
        private bool _isExpanded;

        #endregion

        #region Properties

        public ExportFileTypeEnum FileType
        {
            get { return _fileType; }
            set
            {
                if (_fileType != value)
                {
                    _fileType = value;
                    Notify();
                }
            }
        }

        public string FolderPath
        {
            get { return _folderPath; }
            set
            {
                if (_folderPath != value)
                {
                    _folderPath = value;
                    ExportFileCommand.NotifyCanExecuteChanged();
                    Notify();
                }
            }
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    UpdateData();
                    Notify(nameof(CheckBoxText));
                }
            }
        }

        void UpdateData()
        {
            if (ComputersData != null)
                foreach (var item in ComputersData)
                    item.IsExpanded = IsExpanded;
        }

        public string CheckBoxText
        {
            get { return IsExpanded ? "Collapse all" : "Expand all"; }
        }

        public ObservableCollection<NodeItem> ComputersData
        {
            get { return _computersData; }
            set
            {
                if (_computersData != value)
                {
                    _computersData = value;
                    Notify();
                }
            }
        }

        #endregion

        #region Commands

        public RelayCommand RemoveAppCommand { get; }
        public RelayCommand LoadControl { get; }
        public RelayCommand SelectFolderCommand { get; }
        public RelayCommand ExportFileCommand { get; }

        #endregion

        #region Constructor

        public StartupAppsWindowViewModel(IEnumerable<NodeItem> data)
        {
            _data = data;
            _fileManager = new FileManager();
            FileType = ExportFileTypeEnum.Csv;
            RemoveAppCommand = new RelayCommand(RemoveAppExecute);
            LoadControl = new RelayCommand(LoadControlExecute);
            SelectFolderCommand = new RelayCommand(SelectFolderExecute);
            ExportFileCommand = new RelayCommand(ExportFileExecute, CanExecuteExportFile);
            IsExpanded = true;
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        void Notify([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Execute

        void SelectFolderExecute(object param)
        {
            var dlg = new FolderBrowserDialog();
            var dialogResult = dlg.ShowDialog();
            if (dialogResult == DialogResult.OK)
                FolderPath = dlg.SelectedPath;
        }

        void ExportFileExecute(object param)
        {
            _fileManager.ExportToFile(FolderPath, ComputersData, FileType);
            System.Windows.MessageBox.Show($"Exported To {FolderPath}");
        }

        bool CanExecuteExportFile(object param)
        {
            return !string.IsNullOrEmpty(FolderPath);
        }

        void LoadControlExecute(object param)
        {
            ComputersData = new ObservableCollection<NodeItem>(_data);
            foreach (var item in ComputersData)
                item.IsExpanded = true;
        }

        void RemoveAppExecute(object param)
        {
            if (param is List<object> values)
            {
                var machine = values.FirstOrDefault() as string;
                var appKey = values.LastOrDefault() as string;
                using (registryEditor = new RegistryEditor(machine, RegistryLookupSourceEnum.Machine))
                {
                    try
                    {
                        registryEditor.RemoveStartupAppByKey(appKey);

                        var element = ComputersData.FirstOrDefault(x => x.ComputerName == machine)
                            .Data
                            .FirstOrDefault(x => x.Key == appKey);

                        ComputersData.FirstOrDefault(x => x.ComputerName == machine).Data.Remove(element);
                        Notify(nameof(ComputersData));
                        System.Windows.MessageBox.Show("Removed");
                    }
                    catch (Exception e)
                    {
                        System.Windows.MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        #endregion 
    }
}
