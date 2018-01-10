using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.ComponentModel.DataAnnotations;
using XData.Data.Objects;
using XData.Data.OData;
using XData.Data.Xml;

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
        protected XmlModifier Modifier;

        public SettingsService()
            : this(ConfigurationManager.ConnectionStrings[0].Name)
        {
        }

        public SettingsService(string name)
        {
            Name = name;
            Schema = new SchemaProvider(name).GetSchema();
            ODataQuerier = ODataQuerier<XElement>.Create(Name, Schema);
            Modifier = XmlModifier.Create(name, Schema);
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
            initialPasswordSettings.InitialPassword = element.Element("Value").Value;
            return initialPasswordSettings;
        }

        public void SetMembershipSettings(MembershipSettings value)
        {
            Validate(value);

            IEnumerable<XElement> elements = ODataQuerier.GetCollection("Setting", null, "Catalog eq 'Membership'", null);
            XElement xSettings = new XElement("Settings");
            xSettings.Add(elements);

            XElement xItem = xSettings.Elements().First(x => x.Element("Name").Value == "PasswordFormat");
            xItem.SetElementValue("Value", value.PasswordFormat);

            xItem = xSettings.Elements().First(x => x.Element("Name").Value == "PasswordCrypto");
            xItem.SetElementValue("Value", value.PasswordCrypto);

            xItem = xSettings.Elements().First(x => x.Element("Name").Value == "PasswordHash");
            xItem.SetElementValue("Value", value.PasswordHash);

            xItem = xSettings.Elements().First(x => x.Element("Name").Value == "MaxInvalidPasswordAttempts");
            xItem.SetElementValue("Value", value.MaxInvalidPasswordAttempts);

            xItem = xSettings.Elements().First(x => x.Element("Name").Value == "PasswordAttemptWindow");
            xItem.SetElementValue("Value", value.PasswordAttemptWindow);

            xItem = xSettings.Elements().First(x => x.Element("Name").Value == "MinRequiredPasswordLength");
            xItem.SetElementValue("Value", value.MinRequiredPasswordLength);

            xItem = xSettings.Elements().First(x => x.Element("Name").Value == "MinRequiredNonAlphanumericCharacters");
            xItem.SetElementValue("Value", value.MinRequiredNonAlphanumericCharacters);

            xItem = xSettings.Elements().First(x => x.Element("Name").Value == "PasswordStrengthRegularExpression");
            xItem.SetElementValue("Value", value.PasswordStrengthRegularExpression);

            Modifier.Update(xSettings);
        }

        protected void Validate(MembershipSettings value)
        {
            StringBuilder sb = new StringBuilder();
            if (value.PasswordFormat < 0) sb.AppendLine("The PasswordFormat must be one of {Clear, Hashed, Encrypted}");
            if (value.PasswordCrypto < 0) sb.AppendLine("The PasswordCrypto must be one of {Aes, DES, RC2, Rijndael, TripleDES}");
            if (value.PasswordHash < 0) sb.AppendLine("The PasswordHash must be one of {MD5, SHA1, SHA256, SHA384, SHA512}");
            if (value.MaxInvalidPasswordAttempts < 0) sb.AppendLine("The MaxInvalidPasswordAttempts must be a non-negative number");
            if (value.PasswordAttemptWindow < 0) sb.AppendLine("The PasswordAttemptWindow must be a non-negative number");
            if (value.MinRequiredPasswordLength < 0) sb.AppendLine("The MinRequiredPasswordLength must be a non-negative number");
            if (value.MinRequiredNonAlphanumericCharacters < 0) sb.AppendLine("The MinRequiredNonAlphanumericCharacters must be a non-negative number");

            string errorMessage = sb.ToString();
            if (!string.IsNullOrWhiteSpace(errorMessage)) throw ValidationHelper.CreateValidationException(errorMessage);
        }

        public void SetInitialPasswordSettings(InitialPasswordSettings value)
        {
            Validate(value);

            IEnumerable<XElement> elements = ODataQuerier.GetCollection("Setting", null, "Catalog eq 'InitialPassword'", null);
            XElement element = elements.First();
            element.SetElementValue("InitialPassword", value.InitialPassword);
            Modifier.Update(element);
        }

        protected void Validate(InitialPasswordSettings value)
        {
            if (string.IsNullOrWhiteSpace(value.InitialPassword))
            {
                throw ValidationHelper.CreateValidationException("The InitialPassword is required and cannot be empty");
            }
            if (value.InitialPassword.Length < 6 || value.InitialPassword.Length >= 20)
            {
                throw ValidationHelper.CreateValidationException("The InitialPassword must be at least 6 and not more than 20 characters long");
            }
        }


    }
}
