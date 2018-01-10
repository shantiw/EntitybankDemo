using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using XData.Windows.Interactivity;

namespace XData.Windows.ViewModels
{
    public abstract class ViewModel : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual DependencyObject View { get; set; }

        protected MessageBoxResult ShowMessage(string messageBoxText)
        {
            return ShowMessageBox.Show(messageBoxText, View);
        }

        protected MessageBoxResult ShowMessage(string messageBoxText, MessageBoxButton button)
        {
            return ShowMessageBox.Show(messageBoxText, button, View);
        }

        protected MessageBoxResult ShowMessage(string messageBoxText, MessageBoxButton button, MessageBoxImage icon)
        {
            return ShowMessageBox.Show(messageBoxText, button, icon, View);
        }

        protected MessageBoxResult ShowMessage(string messageBoxText, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult)
        {
            return ShowMessageBox.Show(messageBoxText, button, icon, defaultResult, View);
        }

        protected MessageBoxResult ShowMessage(string messageBoxText, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult, MessageBoxOptions options)
        {
            return ShowMessageBox.Show(messageBoxText, button, icon, defaultResult, options, View);
        }

        public void Dispose()
        {
            if (View != null) // as unmanaged object
            {
                View = null;
            }
        }

        ~ViewModel()
        {
            Dispose();
        }


    }
}
