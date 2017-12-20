using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using XData.Http.Client;

namespace XData.Windows.ViewModels
{
    public class ChangePasswordViewModel : ViewModel
    {
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

        private string _newPassword;
        public string NewPassword
        {
            get { return _newPassword; }
            set
            {
                if (_newPassword != value)
                {
                    _newPassword = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _confirmPassword;
        public string ConfirmPassword
        {
            get { return _confirmPassword; }
            set
            {
                if (_confirmPassword != value)
                {
                    _confirmPassword = value;
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

        public ICommand ChangeCommand { get; protected set; }

        public ICommand CancelCommand { get; protected set; }

        public ChangePasswordViewModel()
        {
            ChangeCommand = new DelegateCommand((_) =>
            {
                if (string.IsNullOrWhiteSpace(Password))
                {
                    ShowMessage("The password is required and cannot be empty");
                    return;
                }
                if (string.IsNullOrWhiteSpace(NewPassword))
                {
                    ShowMessage("The new password is required and cannot be empty");
                    return;
                }
                if (string.IsNullOrWhiteSpace(ConfirmPassword))
                {
                    ShowMessage("The confirm password is required and cannot be empty");
                    return;
                }
                if (NewPassword.Length < 6 || NewPassword.Length > 20)
                {
                    ShowMessage("The new password must be at least 6 and not more than 20 characters long");
                    return;
                }
                if (NewPassword != ConfirmPassword)
                {
                    ShowMessage("The new password and its confirm are not the same");
                    return;
                }

                if (AppSecurity.ChangePassword(Password, NewPassword, out string errorMessage))
                {
                    DialogResult = true;
                }
                else
                {
                    ShowMessage(errorMessage);
                }
            });

            CancelCommand = new DelegateCommand((_) =>
            {
                DialogResult = false;
            });
        }


    }
}
