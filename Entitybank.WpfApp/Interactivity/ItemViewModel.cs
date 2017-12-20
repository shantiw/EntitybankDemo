using System.Xml.Linq;

namespace XData.Windows.ViewModels
{
    public class ItemViewModel : ViewModel
    {
        private XElement _item;
        public XElement Item
        {
            get
            {
                return _item;
            }
            set
            {
                if (_item != value)
                {
                    _item = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
