using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RStyleTranslator
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName="")
            => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        
    }
}
