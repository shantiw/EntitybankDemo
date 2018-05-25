using System.Xml.Linq;

namespace XData.Client.Models
{
    public interface IItemEditorModel
    {
        XElement GetDefault();
        XElement GetEdit(XElement value);
        XElement Create(XElement value, out string errorMessage);
        bool Update(XElement value, out string errorMessage);
    }
}
