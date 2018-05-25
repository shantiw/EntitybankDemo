using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using XData.Client.Models;
using XData.Windows.Views;

namespace XData.Windows.ViewModels
{
    public class EmployeesViewModel : ItemsViewModel
    {
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _gender;
        public string Gender
        {
            get => _gender;
            set
            {
                if (_gender != value)
                {
                    _gender = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _contact;
        public string Contact
        {
            get => _contact;
            set
            {
                if (_contact != value)
                {
                    _contact = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand ClearGenderCommand { get; protected set; }

        public ICommand QueryCommand { get; protected set; }

        public ICommand CreateCommand { get; protected set; }
        public ICommand EditCommand { get; protected set; }
        public ICommand DeleteCommand { get; protected set; }
        public ICommand DetailsCommand { get; protected set; }

        protected EmployeesModel Model = new EmployeesModel();

        public EmployeesViewModel()
        {
            ClearGenderCommand = new DelegateCommand((_) =>
            {
                Gender = null;
            });

            QueryCommand = new DelegateCommand((_) =>
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    { "Name", Name },
                    { "Gender", Gender },
                    { "Contact", Contact }
                };

                IEnumerable<XElement> elements = Model.GetCollection(parameters);
                Collection = new ObservableCollection<XElement>(elements);
            });

            CreateCommand = new DelegateCommand((_) =>
             {
                 Window window = new EmployeeEditor();
                 ItemEditorViewModel viewModel = new ItemEditorViewModel(Model)
                 {
                     View = window
                 };
                 window.DataContext = viewModel;
                 if (window.ShowDialog() == true)
                 {
                     XElement item = Model.GetItem(viewModel.Item);
                     Collection.Insert(0, item);
                     SelectedItem = item;
                 }
             });

            EditCommand = new DelegateCommand((_) =>
            {
                Window window = new EmployeeEditor();
                ItemEditorViewModel viewModel = new ItemEditorViewModel(Model, SelectedItem)
                {
                    View = window
                };
                window.DataContext = viewModel;
                if (window.ShowDialog() == true)
                {
                    // refresh
                    XElement item = Model.GetItem(SelectedItem);
                    foreach (XElement xValue in item.Elements())
                    {
                        SelectedItem.SetElementValue(xValue.Name, xValue.Value);
                    }
                }
            }, (_) => SelectedItem != null);

            DeleteCommand = new DelegateCommand((_) =>
            {
                if (ShowMessage("Are you sure you want to delete?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    if (Model.Delete(SelectedItem, out string errorMessage))
                    {
                        Collection.Remove(SelectedItem);
                    }
                    else
                    {
                        ShowMessage(errorMessage);
                    }
                }
            }, (_) => SelectedItem != null);

            DetailsCommand = new DelegateCommand((_) =>
            {
                Window window = new EmployeeDetails();
                ItemDetailsViewModel viewModel = new ItemDetailsViewModel(Model, SelectedItem)
                {
                    View = window
                };
                window.DataContext = viewModel;
                window.ShowDialog();
            }, (_) => SelectedItem != null);

            PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
            {
                if (e.PropertyName == "SelectedItem")
                {
                    (EditCommand as DelegateCommand).RaiseCanExecuteChanged();
                    (DeleteCommand as DelegateCommand).RaiseCanExecuteChanged();
                    (DetailsCommand as DelegateCommand).RaiseCanExecuteChanged();
                }
            };

            QueryCommand.Execute(null);
        }


    }
}
