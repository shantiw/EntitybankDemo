using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace XData.Windows.Interactivity
{
    public static partial class PasswordBoxAttachedProperties
    {
        public static readonly DependencyProperty PasswordProperty = DependencyProperty.RegisterAttached("Password", typeof(string),
            typeof(PasswordBoxAttachedProperties),
            new UIPropertyMetadata(string.Empty, // replace null with string.Empty
                OnPasswordPropertyChanged));

        // upate the passwordBox when the buffer changed
        private static void OnPasswordPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var passwordBox = d as PasswordBox;
            passwordBox.PasswordChanged -= PasswordChanged;
            if (passwordBox != null)
            {
                passwordBox.Password = e.NewValue == null ? string.Empty : e.NewValue.ToString();
            }
            passwordBox.PasswordChanged += PasswordChanged;
        }

        public static string GetPassword(DependencyObject obj)
        {
            return (string)obj.GetValue(PasswordProperty);
        }

        public static void SetPassword(DependencyObject obj, string value)
        {
            obj.SetValue(PasswordProperty, value);
        }

        // update the buffer when the passwordBox changed
        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = (PasswordBox)sender;
            if (GetPassword(passwordBox) != passwordBox.Password)
            {
                SetPassword(passwordBox, passwordBox.Password);
            }
            SetPasswordBoxSelection(passwordBox, passwordBox.Password.Length + 1, 0);
        }

        private static void SetPasswordBoxSelection(PasswordBox passwordBox, int start, int length)
        {
            var select = passwordBox.GetType().GetMethod("Select", BindingFlags.Instance | BindingFlags.NonPublic);
            select.Invoke(passwordBox, new object[] { start, length });
        }
    }
}
