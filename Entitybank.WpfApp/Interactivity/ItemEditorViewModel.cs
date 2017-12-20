using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using XData.Client.Models;

namespace XData.Windows.ViewModels
{
    public class ItemEditorViewModel : ItemViewModel
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

        public ICommand OKCommand { get; protected set; }
        public ICommand CancelCommand { get; protected set; }

        protected IItemEditorModel Model { get; private set; }

        public ItemEditorViewModel(IItemEditorModel model) : this(model, null)
        {
        }

        public ItemEditorViewModel(IItemEditorModel model, XElement item)
        {
            Model = model;

            if (item == null)
            {
                Item = Model.GetDefault();
            }
            else
            {
                Item = Model.GetEdit(item);
            }

            OKCommand = new DelegateCommand((_) =>
            {
                if (item == null)
                {
                    XElement element = Model.Create(Item, out string errorMessage);
                    if (element != null)
                    {
                        // set primary key
                        foreach (XElement xValue in element.Elements())
                        {
                            Item.SetElementValue(xValue.Name, xValue.Value);
                        }
                        DialogResult = true;
                    }
                    else
                    {
                        ShowMessage(errorMessage);
                    }
                }
                else
                {
                    if (Model.Update(Item, out string errorMessage))
                    {
                        DialogResult = true;
                    }
                    else
                    {
                        ShowMessage(errorMessage);
                    }
                }
            });

            CancelCommand = new DelegateCommand((_) =>
            {
                DialogResult = false;
            });
        }


    }
}
