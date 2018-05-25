using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using XData.Http.Client;

namespace XData.Client.Models
{
    public class EmployeesModel : IItemEditorModel, IItemDetailsModel
    {
        private XmlClient XmlClient;

        public EmployeesModel()
        {
            string baseAddress = ConfigurationManager.AppSettings["BaseAddress"];
            XmlClient = XmlClientManager.GetClient(baseAddress);
        }

        public IEnumerable<XElement> GetCollection(Dictionary<string, string> parameters)
        {
            parameters.Add("key", "Employees");
            return XmlClient.GetCollection(parameters).Elements();
        }

        public XElement GetItem(XElement value)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "key", "Employee" },
                { "id", value.Element("Id").Value}
            };
            return XmlClient.Find(parameters);
        }

        public XElement GetDefault()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "key", "EmployeeDefault" }
            };
            return XmlClient.GetDefault(parameters);
        }

        public XElement GetEdit(XElement value)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "key", "EmployeeEdit" },
                { "id", value.Element("Id").Value}
            };
            return XmlClient.Find(parameters);
        }

        public XElement GetDetails(XElement value)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "key", "EmployeeDetails" },
                { "id", value.Element("Id").Value}
            };
            return XmlClient.Find(parameters);
        }

        public XElement Create(XElement value, out string errorMessage)
        {
            string validationResult = ValidateOnCreating(value);
            if (!string.IsNullOrWhiteSpace(validationResult))
            {
                errorMessage = validationResult;
                return null;
            }

            return XmlClient.Create(value, out errorMessage);
        }

        public bool Update(XElement value, out string errorMessage)
        {
            string validationResult = ValidateOnUpdating(value);
            if (!string.IsNullOrWhiteSpace(validationResult))
            {
                errorMessage = validationResult;
                return false;
            }

            XmlClient.Update(value, out errorMessage);
            return string.IsNullOrWhiteSpace(errorMessage);
        }

        protected string ValidateOnCreating(XElement value)
        {
            return Validate(value);
        }

        protected string ValidateOnUpdating(XElement value)
        {
            return Validate(value);
        }

        private string Validate(XElement value)
        {
            if (string.IsNullOrWhiteSpace(value.Element("Name").Value))
            {
                return "The name is required and cannot be empty";
            }
            return null;
        }

        public bool Delete(XElement value, out string errorMessage)
        {
            XmlClient.Delete(value, out errorMessage);
            return string.IsNullOrWhiteSpace(errorMessage);
        }


    }
}
