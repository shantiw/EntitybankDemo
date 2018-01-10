using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
using XData.Client.Models;
using XData.Windows.Views;

namespace XData.Windows.ViewModels
{
    public class MainViewModel : ViewModel
    {
        private DependencyObject _content = new DefaultUserControl();
        public DependencyObject Content
        {
            get => _content;
            set
            {
                if (_content != value)
                {
                    _content = value;
                }
                OnPropertyChanged();
            }
        }

        public XElement User { get => AppSecurity.User; }

        public ICommand ChangePasswordCommand { get; protected set; }

        public ICommand ExitCommand { get; protected set; }

        public ICommand EmployeesCommand { get; protected set; }

        public MainViewModel()
        {
            ChangePasswordCommand = new DelegateCommand((_) =>
            {
                Window window = new ChangePasswordWindow();
                ViewModel viewModel = new ChangePasswordViewModel
                {
                    View = window
                };
                window.DataContext = viewModel;
                window.ShowDialog();
            });

            ExitCommand = new DelegateCommand((_) =>
            {
                //Application.Current.MainWindow.Close();
				Application.Current.Shutdown();
            });

            EmployeesCommand = new DelegateCommand((_) =>
            {
                UserControl control = new EmployeesUserControl();
                ViewModel viewModel = new EmployeesViewModel
                {
                    View = control
                };
                control.DataContext = viewModel;

                Content = control;
            });
        }


    }
}
