using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace XData.Windows.Interactivity
{
    public static class ShowMessageBox
    {
        public static MessageBoxResult Show(string messageBoxText, DependencyObject view = null)
        {
            Window window = (view == null) ? null : Window.GetWindow(view);
            if (window == null)
            {
                return MessageBox.Show(messageBoxText);
            }
            else
            {
                return MessageBox.Show(window, messageBoxText, window.Title);
            }
        }

        public static MessageBoxResult Show(string messageBoxText, MessageBoxButton button, DependencyObject view)
        {
            Window window = Window.GetWindow(view);
            return MessageBox.Show(messageBoxText, window.Title, button);
        }

        public static MessageBoxResult Show(string messageBoxText, MessageBoxButton button, MessageBoxImage icon, DependencyObject view)
        {
            Window window = Window.GetWindow(view);
            return MessageBox.Show(messageBoxText, window.Title, button, icon);
        }

        public static MessageBoxResult Show(string messageBoxText, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult, DependencyObject view)
        {
            Window window = Window.GetWindow(view);
            return MessageBox.Show(messageBoxText, window.Title, button, icon, defaultResult);
        }

        public static MessageBoxResult Show(string messageBoxText, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult, MessageBoxOptions options, DependencyObject view)
        {
            Window window = Window.GetWindow(view);
            return MessageBox.Show(messageBoxText, window.Title, button, icon, defaultResult, options);
        }


    }
}
