using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WPFAppRecog.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void Set<T>(ref T storage, T value, [CallerMemberName] string callerMemberName = null)
        {
            if (Equals(storage, value) == false)
            {
                storage = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(callerMemberName));
            }
        }

        protected void FireEventChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool IsDesignTime
        {
            get
            {
                return LicenseManager.UsageMode == LicenseUsageMode.Designtime;
            }
        }
    }
}
