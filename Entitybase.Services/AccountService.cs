using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using XData.Data.Modification;
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
        protected Modifier<XElement> Modifier;

        public AccountService()
            : this(ConfigurationManager.ConnectionStrings[0].Name)
        {
        }

        public AccountService(string name)
        {
            Name = name;
            Schema = new SchemaProvider(name).GetSchema();
            ODataQuerier = ODataQuerier<XElement>.Create(Name, Schema);
            Modifier = XmlModifierFactory.Create(name, Schema);
        }

        public XElement GetLoginedUser(string userName)
        {
            List<KeyValuePair<string, string>> keyValues = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("name", "user-info"),
                new KeyValuePair<string, string>("UserName", userName)
            };

            return new XmlService(Name, keyValues).Get();
        }

        public bool Login(string userName, string password, out string errorMessage)
        {
            bool result = false;

            XElement xSecurityEntry = ThreadDataStore.RequestInfo.CreateSecurityEntry(ODataQuerier);
            xSecurityEntry.SetElementValue("Operation", "Login");

            string passwordValue = null;

            XElement xUser = GetUser(userName);
            if (xUser == null)
            {
                passwordValue = password;
                errorMessage = "The user name or password is incorrect";
            }
            else
            {
                if (ComparePassword(xUser, password))
                {
                    if (xUser.Element("IsDisabled").Value == true.ToString())
                    {
                        errorMessage = "Your account has been disabled, Please contact the administrator";
                    }
                    else if (xUser.Element("IsLockedOut").Value == true.ToString())
                    {
                        errorMessage = "Your account has been locked, Please contact the administrator";
                    }
                    else
                    {
                        errorMessage = null;
                        result = true;
                        UpdateLockedOutState(xUser, true);
                    }
                    xSecurityEntry.SetElementValue("UserId", xUser.Element("Id").Value);
                    xSecurityEntry.SetElementValue("CreatedUserId", xUser.Element("Id").Value);

                    XElement xLoginedUser = GetLoginedUser(userName).Element("element").Elements().First().Elements().First();
                    xSecurityEntry.SetElementValue("CreatorName", xLoginedUser.Element("Name").Value);
                }
                else
                {
                    passwordValue = password;
                    errorMessage = "The user name or password is incorrect";
                    UpdateLockedOutState(xUser, false);
                }
            }

            string contents = string.Format("UserName:{0}", userName) +
                (passwordValue == null ? string.Empty : string.Format(",Password:{0}", passwordValue));
            xSecurityEntry.SetElementValue("Contents", contents);
            xSecurityEntry.SetElementValue("IsFailed", !result);
            xSecurityEntry.SetElementValue("ErrorMessage", errorMessage);

            Modifier.AppendCreate(xSecurityEntry);
            Modifier.Persist();
            Modifier.Clear();

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
            if (xSecurityEntry.Element("CreatedUserId") != null)
            {
                xSecurityEntry.SetElementValue("UserId", xSecurityEntry.Element("CreatedUserId").Value);
            }
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
                StringBuilder sb = new StringBuilder();
                foreach (string errMessage in errorMessages)
                {
                    sb.AppendLine(errMessage);
                }
                xSecurityEntry.SetElementValue("Contents", string.Format("NewPassword:{0}", newPassword));
                xSecurityEntry.SetElementValue("ErrorMessage", sb.ToString());
                xSecurityEntry.SetElementValue("IsFailed", true);
                if (xSecurityEntry.Element("CreatedUserId") != null)
                {
                    xSecurityEntry.SetElementValue("UserId", xSecurityEntry.Element("CreatedUserId").Value);
                }
                Modifier.Create(xSecurityEntry);

                throw ValidationHelper.CreateValidationException("NewPassword", errorMessages);
            }

            string errorMessage = null;
            XElement xUser = GetUser(Thread.CurrentPrincipal.Identity.Name);
            if (ComparePassword(xUser, password))
            {
                if (xUser.Element("IsDisabled").Value == true.ToString())
                {
                    errorMessage = "Your account has been disabled, Please contact the administrator";
                }
                else if (xUser.Element("IsLockedOut").Value == true.ToString())
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
            xUser.SetElementValue("Password", encryptedPassword);
            xUser.SetElementValue("PasswordCrypto", crypto);
            xUser.SetElementValue("PasswordKey", key ?? string.Empty);
            xUser.SetElementValue("PasswordIV", iv ?? string.Empty);

            DateTime now = ODataQuerier.GetNow();
            string nowString = new DotNETDateFormatter().Format(now);
            xUser.SetElementValue("LastPasswordChangedDate", nowString);

            Modifier.AppendUpdate(xUser);
            Modifier.AppendCreate(xSecurityEntry);
            Modifier.Persist();
            Modifier.Clear();
        }

        public bool IsInRole(string userName, string roleName)
        {
            IEnumerable<XElement> elements = ODataQuerier.GetCollection("UserRole", null, "LoweredUserName eq @p1 and LoweredRoleName eq @p2", null,
                new Dictionary<string, object>()
                {
                    { "@p1", userName.ToLower() },
                    { "@p2", roleName.ToLower() }
                });
            return elements.Count() > 0;
        }

        protected XElement GetUser(string userName)
        {
            IEnumerable<XElement> elements = ODataQuerier.GetCollection("User", null, "LoweredUserName eq @p1", null,
                new Dictionary<string, object>() { { "@p1", userName.ToLower() } });
            return elements.FirstOrDefault();
        }


    }
}
