using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using XData.Data.Objects;
using XData.Data.OData;
using XData.Data.Xml;

namespace XData.Data.Services
{
    public class UsersService
    {
        protected readonly string Name;
        protected readonly XElement Schema;
        protected ODataQuerier<XElement> ODataQuerier;
        protected XmlModifier Modifier;

        public UsersService()
            : this(ConfigurationManager.ConnectionStrings[0].Name)
        {
        }

        public UsersService(string name)
        {
            Name = name;
            Schema = new SchemaProvider(name).GetSchema();
            ODataQuerier = ODataQuerier<XElement>.Create(Name, Schema);
            Modifier = XmlModifier.Create(name, Schema);
        }

        public void Create(int employeeId, string userName, bool isDisabled, IEnumerable<int> roleIds)
        {
            SettingsService settingsService = new SettingsService(Name);
            MembershipSettings membershipSettings = settingsService.GetMembershipSettings();
            string password = settingsService.GetInitialPasswordSettings().InitialPassword;
            string encryptedPassword = PasswordSecurity.EncryptPassword(membershipSettings, password, out int crypto, out string key, out string iv);

            XElement xUser = new XElement("User");
            xUser.SetElementValue("EmployeeId", employeeId);
            xUser.SetElementValue("UserName", userName);
            xUser.SetElementValue("LoweredUserName", userName.ToLower());
            xUser.SetElementValue("IsDisabled", isDisabled);
            xUser.SetElementValue("Password", encryptedPassword);
            xUser.SetElementValue("PasswordCrypto", crypto);
            xUser.SetElementValue("PasswordKey", key);
            xUser.SetElementValue("PasswordIV", iv);

            XElement xRoles = new XElement("Roles");
            foreach (int roleId in roleIds)
            {
                XElement xRole = new XElement("Role");
                xRole.SetElementValue("Id", roleId);
                xRoles.Add(xRole);
            }

            if (xRoles.HasElements)
            {
                xUser.Add(xRoles);
            }

            Modifier.Create(xUser);
        }

        public void GrantRoles(int id, IEnumerable<int> roleIds)
        {
            XElement xUser = new XElement("User");
            xUser.SetElementValue("Id", id);

            XElement xRoles = new XElement("Roles");
            foreach (int roleId in roleIds)
            {
                XElement xRole = new XElement("Role");
                xRole.SetElementValue("Id", roleId);
                xRoles.Add(xRole);
            }
            xUser.Add(xRoles);

            Modifier.Update(xUser);
        }

        public void ResetPassword(int id, string password)
        {
            string userName = GetUserName(id);

            MembershipSettings membershipSettings = new SettingsService(Name).GetMembershipSettings();
            string encryptedPassword = PasswordSecurity.EncryptPassword(membershipSettings, password, out int crypto, out string key, out string iv);

            XElement xUser = new XElement("User");
            xUser.SetElementValue("Id", id);
            xUser.SetElementValue("Password", encryptedPassword);
            xUser.SetElementValue("PasswordCrypto", crypto);
            xUser.SetElementValue("PasswordKey", key);
            xUser.SetElementValue("PasswordIV", iv);

            XElement xSecurityEntry = ThreadDataStore.RequestInfo.CreateSecurityEntry(ODataQuerier);
            xSecurityEntry.SetElementValue("Operation", "ResetPassword");
            xSecurityEntry.SetElementValue("Contents", string.Format("UserId:{0},UserName:{1}", id, userName));
            xSecurityEntry.SetElementValue("UserId", id);

            Update(xUser, xSecurityEntry);
        }

        public void SetStatus(int id, bool isDisabled)
        {
            string userName = GetUserName(id);

            XElement xUser = new XElement("User");
            xUser.SetElementValue("Id", id);
            xUser.SetElementValue("IsDisabled", isDisabled);

            XElement xSecurityEntry = ThreadDataStore.RequestInfo.CreateSecurityEntry(ODataQuerier);
            xSecurityEntry.SetElementValue("Operation", isDisabled ? "Disable" : "Enable");
            xSecurityEntry.SetElementValue("Contents", string.Format("UserId:{0},UserName:{1}", id, userName));
            xSecurityEntry.SetElementValue("UserId", id);

            Update(xUser, xSecurityEntry);
        }

        public void Unlock(int id)
        {
            string userName = GetUserName(id);

            XElement xUser = new XElement("User");
            xUser.SetElementValue("Id", id);
            xUser.SetElementValue("IsLockedOut", false);

            XElement xSecurityEntry = ThreadDataStore.RequestInfo.CreateSecurityEntry(ODataQuerier);
            xSecurityEntry.SetElementValue("Operation", "Unlock");
            xSecurityEntry.SetElementValue("Contents", string.Format("UserId:{0},UserName:{1}", id, userName));
            xSecurityEntry.SetElementValue("UserId", id);

            Update(xUser, xSecurityEntry);
        }

        protected string GetUserName(int id)
        {
            XElement xUser = ODataQuerier.Find("User", new string[] { id.ToString() }, "UserName");
            return xUser.Element("UserName").Value;
        }

        protected void Update(XElement xUser, XElement xSecurityEntry)
        {
            Modifier.AppendUpdate(xUser);
            Modifier.AppendCreate(xSecurityEntry);
            Modifier.Persist();
            Modifier.Clear();
        }


    }
}
