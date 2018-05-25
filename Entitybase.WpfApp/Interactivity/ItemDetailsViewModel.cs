using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using XData.Client.Models;

namespace XData.Windows.ViewModels
{
    public class ItemDetailsViewModel : ItemViewModel
    {
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

        public ICommand CloseCommand { get; protected set; }

        protected IItemDetailsModel Model { get; private set; }

        public ItemDetailsViewModel(IItemDetailsModel model, XElement item)
        {
            Model = model;

            Item = Model.GetDetails(item);

            CloseCommand = new DelegateCommand((_) =>
            {
                DialogResult = true;
            });
        }


    }
}
