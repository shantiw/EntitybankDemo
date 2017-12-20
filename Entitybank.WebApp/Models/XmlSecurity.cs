using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Web.Security;
using System.Xml.Linq;
using XData.Data.Services;

namespace XData.Web.Models
{
    public static class XmlSecurity
    {
        private static AccountService AccountService { get => new AccountService(); }
        private static AsymmetricCryptoService AsymmetricCryptoService { get => new AsymmetricCryptoService(); }

        public static XElement Login(XElement value)
        {
            string userName = value.Element("UserName").Value;
            string password = value.Element("Password").Value;
            bool rememberMe = bool.Parse(value.Element("RememberMe").Value);

            userName = AsymmetricCryptoService.Decrypt(userName);
            password = AsymmetricCryptoService.Decrypt(password);

            if (AccountService.Login(userName, password, out string errorMessage))
            {
                FormsAuthentication.SetAuthCookie(userName, rememberMe);
                return AccountService.GetLoginedUser(userName);
            }
            else
            {
                throw ValidationHelper.CreateValidationException(errorMessage);
            }
        }

        public static void Logout()
        {
            AccountService.Logout();
            FormsAuthentication.SignOut();
        }

        public static void ChangePassword(XElement value)
        {
            string password = value.Element("Password").Value;
            string newPassword = value.Element("NewPassword").Value;

            password = AsymmetricCryptoService.Decrypt(password);
            newPassword = AsymmetricCryptoService.Decrypt(newPassword);

            AccountService.ChangePassword(password, newPassword);
        }

        public static XElement GetPublicKey()
        {
            return new XElement("PublicKey", AsymmetricCryptoService.GetPublicKey());
        }


    }
}