using System;
using System.Windows.Input;

namespace WindowsStartupTool.Service.Client
{
    internal class RelayCommand : ICommand
    {
        #region Fields
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;
        #endregion

        #region Constructors
        public RelayCommand(Action<object> execute)
        {
            _execute = execute;
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        #endregion

        #region Methods
        public void NotifyCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region ICommand Implementation
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
        #endregion
    }
}
