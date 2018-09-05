using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WindowsStartupTool.Lib
{
    public class NodeItem : INotifyPropertyChanged
    {
        #region Fields

        private bool _isExpanded;

        #endregion

        #region Properties

        public string ComputerName { get; set; }

        [JsonIgnore]
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    Notify();
                }
            }
        }

        public ObservableCollection<KeyValuePair<string, string>> Data { get; set; }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        void Notify([CallerMemberName] string property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }


        #endregion
    }
}
