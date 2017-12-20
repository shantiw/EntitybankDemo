using System.Xml.Linq;

namespace XData.Client.Models
{
    public interface IItemDetailsModel
    {
        XElement GetDetails(XElement value);
    }
}
