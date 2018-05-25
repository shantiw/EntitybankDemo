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
    public static class AppSecurity
    {
        private static XmlClient XmlClient;

        static AppSecurity()
        {
            string baseAddress = ConfigurationManager.AppSettings["BaseAddress"];
            XmlClient = XmlClientManager.GetClient(baseAddress);
        }

        public static XElement User { get; private set; }

        public static bool Login(string userName, string password, out string errorMessage)
        {
            User = XmlClient.Login(userName, password, false, out errorMessage);
            return User != null;
        }

        public static void Logout()
        {
            if (User == null) return;

            XmlClient.Logout();
            User = null;
        }

        public static bool ChangePassword(string password, string newPassword, out string errorMessage)
        {
            XmlClient.ChangePassword(password, newPassword, out errorMessage);
            return string.IsNullOrWhiteSpace(errorMessage);
        }


    }
}
