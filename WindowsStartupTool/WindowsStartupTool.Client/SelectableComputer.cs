﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WindowsStartupTool.Client
{
    public class SelectableComputer : INotifyPropertyChanged
    {
        #region Fields

        private string _name;
        private bool _isSelected;
        private bool _isEnabled;

        #endregion

        #region Properties

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                Notify();
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                Notify();
            }
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
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

        #region Override

        public override string ToString()
        {
            return Name ?? string.Empty;
        }

        #endregion
    }
}
