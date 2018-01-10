using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using XData.Data.Security;
using XData.Net.Http;

namespace XData.Http.Client
{
    public class XmlClient
    {
        public const string AccountUri = "xml/Account";

        public const string XmlUri = "xml";

        protected readonly XmlHttpClient XmlHttpClient;

        public XmlClient(string baseAddress)
        {
            XmlHttpClient = new XmlHttpClient(baseAddress);
        }

        public XElement Login(string userName, string password, bool rememberMe, out string errorMessage)
        {
            string publicKey = GetPublicKey(out errorMessage);
            if (errorMessage != null) return null;

            XElement value = new XElement("Login");
            value.SetElementValue("UserName", RSACryptor.Encrypt(userName, publicKey));
            value.SetElementValue("Password", RSACryptor.Encrypt(password, publicKey));
            value.SetElementValue("RememberMe", rememberMe);
            XElement result = XmlHttpClient.Post(AccountUri, value);

            errorMessage = result.GetErrorMessage();
            if (errorMessage == null)
            {
                XElement user = result.Element("element").Elements().First().Elements().First();
                return user;
            }

            return null;
        }

        public void Logout()
        {
            XmlHttpClient.Delete(AccountUri);
        }

        public void ChangePassword(string password, string newPassword, out string errorMessage)
        {
            string publicKey = GetPublicKey(out errorMessage);
            if (errorMessage != null) return;

            XElement value = new XElement("ChangePassword");
            value.SetElementValue("Password", RSACryptor.Encrypt(password, publicKey));
            value.SetElementValue("NewPassword", RSACryptor.Encrypt(newPassword, publicKey));
            XElement result = XmlHttpClient.Put(AccountUri, value);

            errorMessage = result.GetErrorMessage();
        }

        protected string GetPublicKey(out string errorMessage)
        {
            XElement publicKey = XmlHttpClient.Get(AccountUri);

            errorMessage = publicKey.GetErrorMessage();
            if (errorMessage == null) return publicKey.Value;

            return null;
        }

        public DateTime GetNow()
        {
            XElement element = XmlHttpClient.Get(XmlUri + "/$now");
            return DateTime.Parse(element.Value);
        }

        public DateTime GetUtcNow()
        {
            XElement element = XmlHttpClient.Get(XmlUri + "/$utcnow");
            return DateTime.Parse(element.Value);
        }

        public int Count(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            XElement element = XmlHttpClient.Get(XmlUri + GetQueryString(parameters));
            int count = int.Parse(element.Element("count").Value);
            return count;
        }

        public XElement GetDefault(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            XElement element = XmlHttpClient.Get(XmlUri + GetQueryString(parameters));
            return element.Element("element").Elements().First();
        }

        public XElement Find(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            XElement result = XmlHttpClient.Get(XmlUri + GetQueryString(parameters));
            XElement element = result.Element("element").Elements().First().Elements().First();
            return (element.HasElements) ? element : null;
        }

        public XElement GetCollection(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            XElement element = XmlHttpClient.Get(XmlUri + GetQueryString(parameters));
            return element.Element("element").Elements().First();
        }

        public XElement GetPage(IEnumerable<KeyValuePair<string, string>> parameters, out int count)
        {
            XElement element = XmlHttpClient.Get(XmlUri + GetQueryString(parameters));
            count = int.Parse(element.Element("count").Value);
            return element.Element("element").Elements().First();
        }

        protected string GetQueryString(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            if (parameters.Count() == 0) return string.Empty;
            IEnumerable<string> nameValues = parameters.Select(p => string.Format("{0}={1}", p.Key, p.Value));
            return "?" + string.Join("&", nameValues);
        }

        public XElement Create(XElement value, out string errorMessage)
        {
            XElement result = XmlHttpClient.Post(XmlUri, value);

            errorMessage = result.GetErrorMessage();
            return (errorMessage == null) ? result : null;
        }

        public void Delete(XElement value, out string errorMessage)
        {
            XElement result = XmlHttpClient.Delete(XmlUri, value);
            errorMessage = result.GetErrorMessage();
        }

        public void Update(XElement value, out string errorMessage)
        {
            XElement result = XmlHttpClient.Put(XmlUri, value);
            errorMessage = result.GetErrorMessage();
        }

    }
}
