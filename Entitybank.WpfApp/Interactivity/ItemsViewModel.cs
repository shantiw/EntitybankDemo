using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace XData.Windows.ViewModels
{
    public class ItemsViewModel : ViewModel
    {
        private XElement _selectedItem;
        public XElement SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    OnPropertyChanged();
                }
            }
        }

        private ReadOnlyObservableCollection<XElement> _items;
        public ReadOnlyObservableCollection<XElement> Items
        {
            get => _items;
            private set
            {
                _items = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<XElement> _collection;
        public ObservableCollection<XElement> Collection
        {
            get => _collection;
            set
            {
                if (_collection != value)
                {
                    _collection = value;
                    Items = new ReadOnlyObservableCollection<XElement>(_collection);
                }
            }
        }
    }
}
