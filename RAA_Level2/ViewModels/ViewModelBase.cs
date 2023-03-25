using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RAA_Level2.ViewModels
{
    /// <summary>
    /// Base class that implements INotifyPropertyChanged interface for ViewModels.
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
