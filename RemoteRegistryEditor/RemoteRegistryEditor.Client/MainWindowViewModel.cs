using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RemoteRegistryEditor.Client
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        void Notify([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
