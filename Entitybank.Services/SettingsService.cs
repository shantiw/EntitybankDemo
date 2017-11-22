using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using XData.Data.Objects;
using XData.Data.OData;

namespace XData.Data.Services
{
    public class MembershipSettings
    {
        public int PasswordFormat { get; set; }
        public int PasswordCrypto { get; set; }
        public int PasswordHash { get; set; }
        public int MaxInvalidPasswordAttempts { get; set; }
        public int PasswordAttemptWindow { get; set; }
        public int MinRequiredPasswordLength { get; set; }
        public int MinRequiredNonAlphanumericCharacters { get; set; }
        public string PasswordStrengthRegularExpression { get; set; }
    }

    public class InitialPasswordSettings
    {
        public string InitialPassword { get; set; }
    }

    public class SettingsService
    {
        protected readonly string Name;
        protected readonly XElement Schema;
        protected ODataQuerier<XElement> ODataQuerier;

        public SettingsService()
            : this(ConfigurationManager.ConnectionStrings[0].Name)
        {
        }

        public SettingsService(string name)
        {
            Name = name;
            Schema = GetSchema(name, new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("name", "admin") });
            ODataQuerier = ODataQuerier<XElement>.Create(Name, Schema);
        }

        protected static XElement GetSchema(string name, IEnumerable<KeyValuePair<string, string>> deltaKey)
        {
            SchemaProvider schemaProvider = new SchemaProvider(name);
            return schemaProvider.GetSchema(deltaKey);
        }

        public MembershipSettings GetMembershipSettings()
        {
            MembershipSettings membershipSettings = new MembershipSettings();
            IEnumerable<XElement> elements = ODataQuerier.GetCollection("Setting", null, "Catalog eq 'Membership'", null);
            foreach (XElement element in elements)
            {
                string name = element.Element("Name").Value;
                string value = element.Element("Value").Value;
                switch (name)
                {
                    case "PasswordFormat":
                        membershipSettings.PasswordFormat = int.Parse(value);
                        break;
                    case "PasswordCrypto":
                        membershipSettings.PasswordCrypto = int.Parse(value);
                        break;
                    case "PasswordHash":
                        membershipSettings.PasswordHash = int.Parse(value);
                        break;
                    case "MaxInvalidPasswordAttempts":
                        membershipSettings.MaxInvalidPasswordAttempts = int.Parse(value);
                        break;
                    case "PasswordAttemptWindow":
                        membershipSettings.PasswordAttemptWindow = int.Parse(value);
                        break;
                    case "MinRequiredPasswordLength":
                        membershipSettings.MinRequiredPasswordLength = int.Parse(value);
                        break;
                    case "MinRequiredNonAlphanumericCharacters":
                        membershipSettings.MinRequiredNonAlphanumericCharacters = int.Parse(value);
                        break;
                    case "PasswordStrengthRegularExpression":
                        membershipSettings.PasswordStrengthRegularExpression = value;
                        break;
                }
            }
            return membershipSettings;
        }

        public InitialPasswordSettings GetInitialPasswordSettings()
        {
            InitialPasswordSettings initialPasswordSettings = new InitialPasswordSettings();
            IEnumerable<XElement> elements = ODataQuerier.GetCollection("Setting", null, "Catalog eq 'InitialPassword'", null);
            XElement element = elements.First();
            initialPasswordSettings.InitialPassword = element.Element("initialPasswordSettings").Value;
            return initialPasswordSettings;
        }


    }
}
