using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using WindowsStartupTool.Lib;

namespace WindowsStartupTool.Client.SkipKeysWindow
{
    class SkipKeysWindowViewModel : INotifyPropertyChanged
    {
        #region Fields

        private readonly FileManager _fileManager;
        private ObservableCollection<Node> _names;

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        void Notify([CallerMemberName] string prop = null)
        {
            PropertyChanged?.Invoke(prop, new PropertyChangedEventArgs(prop));
        }

        #endregion

        #region Constructors

        public SkipKeysWindowViewModel()
        {
            _fileManager = new FileManager();

            Names = new ObservableCollection<Node>(_fileManager.GetFileContent().Select(x => new Node { Value = x }));
            SaveCommand = new RelayCommand(SaveExecute);
        }

        #endregion

        #region Properties

        public ObservableCollection<Node> Names
        {
            get { return _names; }
            set
            {
                if (_names != value)
                {
                    _names = value;
                    Notify();
                }
            }
        }

        #endregion

        #region Command
        public RelayCommand SaveCommand { get; }
        #endregion

        #region Execute

        void SaveExecute(object param)
        {
            _fileManager.SaveToFile(Names.Select(x => x.Value));

            Names = new ObservableCollection<Node>(_fileManager.GetFileContent().Select(x => new Node { Value = x }));

            MessageBox.Show("Saved", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion
    }

    #region Classes

    class Node
    {
        public string Value { get; set; }
    }

    #endregion
}
