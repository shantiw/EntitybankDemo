using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using XData.Data.Objects;
using XData.Data.OData;
using XData.Data.Xml;

namespace XData.Data.Services
{
    public class AccountService
    {
        protected readonly string Name;
        protected readonly XElement Schema;
        protected ODataQuerier<XElement> ODataQuerier;
        protected XmlModifier Modifier;

        public AccountService()
            : this(ConfigurationManager.ConnectionStrings[0].Name)
        {
        }

        public AccountService(string name)
        {
            Name = name;
            Schema = GetSchema(name, new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("name", "account") });
            ODataQuerier = ODataQuerier<XElement>.Create(Name, Schema);
            Modifier = XmlModifier.Create(name, Schema);
        }

        public XElement GetLoginedUser(string userName)
        {
            List<KeyValuePair<string, string>> keyValues = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("name", "user-info"),
                new KeyValuePair<string, string>("UserName", userName)
            };

            XElement result = new XmlService(Name, keyValues).Get();
            return result.Element("element").Elements().First().Elements().First();
        }

        public bool Login(string userName, string password, out string errorMessage)
        {
            bool result = false;

            XElement xSecurityEntry = ThreadDataStore.RequestInfo.CreateSecurityEntry(ODataQuerier);
            xSecurityEntry.SetElementValue("Operation", "Login");

            string passwordValue = null;

            XElement user = GetUser(userName);
            if (user == null)
            {
                passwordValue = password;
                errorMessage = "The user name or password is incorrect";
            }
            else
            {
                if (ComparePassword(user, password))
                {
                    if (user.Element("IsDisabled").Value == true.ToString())
                    {
                        errorMessage = "Your account has been disabled, Please contact the administrator";
                    }
                    else if (user.Element("IsLockedOut").Value == true.ToString())
                    {
                        errorMessage = "Your account has been locked, Please contact the administrator";
                    }
                    else
                    {
                        errorMessage = null;
                        result = true;
                        UpdateLockedOutState(user, true);
                    }
                    xSecurityEntry.SetElementValue("CreatedUserId", user.Element("Id").Value);
                    xSecurityEntry.SetElementValue("CreatorName", GetLoginedUser(userName).Element("Name").Value);
                }
                else
                {
                    passwordValue = password;
                    errorMessage = "The user name or password is incorrect";
                    UpdateLockedOutState(user, false);
                }
            }

            string contents = string.Format("UserName:{0}", userName) +
                (passwordValue == null ? string.Empty : string.Format(",Password:{0}", passwordValue));
            xSecurityEntry.SetElementValue("Contents", contents);
            xSecurityEntry.SetElementValue("IsFailed", !result);
            xSecurityEntry.SetElementValue("ErrorMessage", errorMessage);

            Modifier.AppendCreate(xSecurityEntry);
            Modifier.Persist();

            return result;
        }

        protected static bool ComparePassword(XElement user, string password)
        {
            string encryptedPassword = user.Element("Password").Value;
            int crypto = int.Parse(user.Element("PasswordCrypto").Value);
            string key = user.Element("PasswordKey").Value;
            string iv = user.Element("PasswordIV").Value;
            return PasswordSecurity.ComparePassword(encryptedPassword, crypto, key, iv, password);
        }

        protected void UpdateLockedOutState(XElement user, bool isSuccessful)
        {
            DateTime now = ODataQuerier.GetNow();
            string nowString = new DotNETDateFormatter().Format(now);
            SettingsService settingsService = new SettingsService(Name);
            MembershipSettings membershipSettings = settingsService.GetMembershipSettings();

            if (isSuccessful)
            {
                user.SetElementValue("LastLoginDate", nowString);
                user.SetElementValue("FailedPasswordAttemptCount", 0);
            }
            else
            {
                if (membershipSettings.MaxInvalidPasswordAttempts == 0) return;

                int failedCount = int.Parse(user.Element("FailedPasswordAttemptCount").Value);
                if (failedCount > 0)
                {
                    DateTime start = DateTime.Parse(user.Element("FailedPasswordAttemptWindowStart").Value);
                    if ((DateTime.Now - start).TotalMinutes < membershipSettings.PasswordAttemptWindow)
                    {
                        if (failedCount >= membershipSettings.MaxInvalidPasswordAttempts)
                        {
                            user.SetElementValue("IsLockedOut", true.ToString());
                            user.SetElementValue("LastLockoutDate", nowString);
                        }
                        else
                        {
                            failedCount++;
                            user.SetElementValue("FailedPasswordAttemptCount", failedCount);
                        }
                    }
                    else
                    {
                        user.SetElementValue("FailedPasswordAttemptCount", 1);
                        user.SetElementValue("FailedPasswordAttemptWindowStart", nowString);
                    }
                }
                else
                {
                    user.SetElementValue("FailedPasswordAttemptCount", 1);
                    user.SetElementValue("FailedPasswordAttemptWindowStart", nowString);
                }
            }
            Modifier.AppendUpdate(user);
        }

        public void Logout()
        {
            XElement xSecurityEntry = ThreadDataStore.RequestInfo.CreateSecurityEntry(ODataQuerier);
            xSecurityEntry.SetElementValue("Operation", "Logout");
            Modifier.Create(xSecurityEntry);
        }

        public void ChangePassword(string password, string newPassword)
        {
            XElement xSecurityEntry = ThreadDataStore.RequestInfo.CreateSecurityEntry(ODataQuerier);
            xSecurityEntry.SetElementValue("Operation", "ChangePassword");

            SettingsService settingsService = new SettingsService(Name);
            MembershipSettings membershipSettings = settingsService.GetMembershipSettings();

            if (!PasswordSecurity.ValidatePassword(membershipSettings, newPassword, out string[] errorMessages))
            {
                xSecurityEntry.SetElementValue("Contents", string.Format("NewPassword:{0}", newPassword));
                xSecurityEntry.SetElementValue("ErrorMessage", string.Join("\r", errorMessages));
                xSecurityEntry.SetElementValue("IsFailed", true);
                Modifier.Create(xSecurityEntry);

                throw ValidationHelper.CreateValidationException("NewPassword", errorMessages);
            }

            string errorMessage = null;
            XElement user = GetUser(Thread.CurrentPrincipal.Identity.Name);
            if (ComparePassword(user, password))
            {
                if (user.Element("IsDisabled").Value == true.ToString())
                {
                    errorMessage = "Your account has been disabled, Please contact the administrator";
                }
                else if (user.Element("IsLockedOut").Value == true.ToString())
                {
                    errorMessage = "Your account has been locked, Please contact the administrator";
                }
            }
            else
            {
                errorMessage = "The password is incorrect";
            }

            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                xSecurityEntry.SetElementValue("ErrorMessage", errorMessage);
                xSecurityEntry.SetElementValue("IsFailed", true);
                Modifier.Create(xSecurityEntry);
                throw ValidationHelper.CreateValidationException(errorMessage);
            }

            string encryptedPassword = PasswordSecurity.EncryptPassword(membershipSettings, newPassword, out int crypto, out string key, out string iv);
            user.SetElementValue("Password", encryptedPassword);
            user.SetElementValue("PasswordCrypto", crypto);
            user.SetElementValue("PasswordKey", key ?? string.Empty);
            user.SetElementValue("PasswordIV", iv ?? string.Empty);

            DateTime now = ODataQuerier.GetNow();
            string nowString = new DotNETDateFormatter().Format(now);
            user.SetElementValue("LastPasswordChangedDate", nowString);

            Modifier.AppendUpdate(user);
            Modifier.AppendCreate(xSecurityEntry);
            Modifier.Persist();
        }

        protected XElement GetUser(string userName)
        {
            IEnumerable<XElement> elements = ODataQuerier.GetCollection("User", null, "LoweredUserName eq @p1", null,
                new Dictionary<string, object>() { { "@p1", userName.ToLower() } });
            return elements.FirstOrDefault();
        }

        protected static XElement GetSchema(string name, IEnumerable<KeyValuePair<string, string>> deltaKey)
        {
            SchemaProvider schemaProvider = new SchemaProvider(name);
            return schemaProvider.GetSchema(deltaKey);
        }


    }
}
