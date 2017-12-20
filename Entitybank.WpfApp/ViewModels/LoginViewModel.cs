using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using XData.Http.Client;

namespace XData.Windows.ViewModels
{
    public class LoginViewModel : ViewModel
    {
        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool? _dialogResult;
        public bool? DialogResult
        {
            get { return _dialogResult; }
            protected set
            {
                _dialogResult = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginCommand { get; protected set; }

        public ICommand ExitCommand { get; protected set; }

        public LoginViewModel()
        {
            LoginCommand = new DelegateCommand((_) =>
            {
                if (string.IsNullOrWhiteSpace(UserName))
                {
                    ShowMessage("The user name is required and cannot be empty");
                    return;
                }
                if (string.IsNullOrWhiteSpace(Password))
                {
                    ShowMessage("The password is required and cannot be empty");
                    return;
                }

                if (AppSecurity.Login(UserName, Password, out string errorMessage))
                {
                    DialogResult = true;
                }
                else
                {
                    ShowMessage(errorMessage);
                }
            });

            ExitCommand = new DelegateCommand((_) =>
            {
                DialogResult = false;
            });
        }


    }
}
