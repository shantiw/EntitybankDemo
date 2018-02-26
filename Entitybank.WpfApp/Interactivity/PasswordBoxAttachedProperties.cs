using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace XData.Windows.Interactivity
{
    public static partial class PasswordBoxAttachedProperties
    {
        public static readonly DependencyProperty PasswordProperty = DependencyProperty.RegisterAttached("Password", typeof(string),
            typeof(PasswordBoxAttachedProperties),
            new UIPropertyMetadata(string.Empty, OnPasswordPropertyChanged));

        private static void OnPasswordPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox passwordBox)
            {
                passwordBox.Password = e.NewValue == null ? string.Empty : e.NewValue.ToString();
            }
        }

        public static string GetPassword(DependencyObject obj)
        {
            return (string)obj.GetValue(PasswordProperty);
        }

        public static void SetPassword(DependencyObject obj, string value)
        {
            obj.SetValue(PasswordProperty, value);
        }
    }

    public class PasswordBoxBehavior : Behavior<PasswordBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.PasswordChanged += OnPasswordChanged;
        }

        private static void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;

            string password = PasswordBoxAttachedProperties.GetPassword(passwordBox);

            if (passwordBox.Password != password)
            {
                PasswordBoxAttachedProperties.SetPassword(passwordBox, passwordBox.Password ?? string.Empty);
                SetPasswordBoxSelection(passwordBox, passwordBox.Password.Length + 1, 0);
            }
        }

        private static void SetPasswordBoxSelection(PasswordBox passwordBox, int start, int length)
        {
            MethodInfo select = passwordBox.GetType().GetMethod("Select", BindingFlags.Instance | BindingFlags.NonPublic);
            select.Invoke(passwordBox, new object[] { start, length });
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PasswordChanged -= OnPasswordChanged;

            base.OnDetaching();
        }
    }

}
