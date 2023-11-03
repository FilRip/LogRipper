using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace LogRipper.Helpers
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        internal void VerifyPropertyName(string propertyName)
        {
            if (TypeDescriptor.GetProperties(this)[propertyName] != null)
                return;
            Debug.Fail("Invalid property name: " + propertyName);
        }

        internal virtual void OnPropertyChanged([CallerMemberName()] string propertyName = null)
        {
            VerifyPropertyName(propertyName);
            if (PropertyChanged == null)
                return;
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
