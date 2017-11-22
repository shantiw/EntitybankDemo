using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using XData.Data.Helpers;
using XData.Data.Security;

namespace XData.Data.Services
{
    internal static class PasswordSecurity
    {
        public static bool ComparePassword(string encryptedPassword, int crypto, string key, string iv, string password)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                if (string.IsNullOrWhiteSpace(iv))
                {
                    // Clear
                    return password == encryptedPassword;
                }
                else  // Hash
                {
                    Hasher hasher = CreateHasher(crypto);
                    string salt = iv;
                    return hasher.Hash(password, salt) == encryptedPassword;
                }
            }
            else // Crypto
            {
                SymmetricCryptor cryptor = CreateSymmetricCryptor(crypto, key, iv);
                return cryptor.Encrypt(password) == encryptedPassword;
            }
        }

        public static bool ValidatePassword(MembershipSettings membershipSettings, string password, out string[] errorMessages)
        {
            List<string> errors = new List<string>();
            if (password.Length < membershipSettings.MinRequiredPasswordLength)
            {
                string error = string.Format("The new password must be at least {0} characters long", membershipSettings.MinRequiredPasswordLength);
                errors.Add(error);
            }
            if (membershipSettings.MinRequiredNonAlphanumericCharacters > 0)
            {
                int nonAlphanumericCount = 0;
                foreach (char c in password)
                {
                    if (char.IsLetterOrDigit(c)) continue;
                    nonAlphanumericCount++;
                }
                if (nonAlphanumericCount < membershipSettings.MinRequiredNonAlphanumericCharacters)
                {
                    string error = string.Format("The new password requires at least {0} non-letter or digit characters", membershipSettings.MinRequiredNonAlphanumericCharacters);
                    errors.Add(error);
                }
            }
            if (!string.IsNullOrWhiteSpace(membershipSettings.PasswordStrengthRegularExpression))
            {
                Match match = Regex.Match(password, membershipSettings.PasswordStrengthRegularExpression);
                if (!match.Success || match.Value != password)
                {
                    string error = string.Format("The new password must match the specified regular expression '{0}'", membershipSettings.PasswordStrengthRegularExpression);
                    errors.Add(error);
                }
            }
            errorMessages = errors.ToArray();
            return errorMessages.Length == 0;
        }

        public static string EncryptPassword(MembershipSettings membershipSettings, string password, out int crypto, out string key, out string iv)
        {
            switch (membershipSettings.PasswordFormat)
            {
                // Clear
                case 0:
                    crypto = 0;
                    key = null;
                    iv = null;
                    return password;
                // Hashed
                case 1:
                    string salt = Convert.ToBase64String(new RandomNumGenerator().Generate());
                    crypto = membershipSettings.PasswordHash;
                    key = null;
                    iv = salt;
                    Hasher hasher = CreateHasher(crypto);
                    return hasher.Hash(password, salt);
                // Encrypted
                case 2:
                    crypto = membershipSettings.PasswordCrypto;
                    SymmetricCryptor cryptor = CreateSymmetricCryptor(crypto);
                    key = Convert.ToBase64String(cryptor.Key);
                    iv = Convert.ToBase64String(cryptor.IV);
                    return cryptor.Encrypt(password);
                default:
                    throw new NotSupportedException(membershipSettings.PasswordFormat.ToString()); // never
            }
        }

        private static Hasher CreateHasher(int crypto)
        {
            Hasher hasher = null;
            switch (crypto)
            {
                case (int)HashName.MD5:
                    hasher = new MD5Hasher();
                    break;
                case (int)HashName.SHA1:
                    hasher = new SHA1Hasher();
                    break;
                case (int)HashName.SHA256:
                    hasher = new SHA256Hasher();
                    break;
                case (int)HashName.SHA384:
                    hasher = new SHA384Hasher();
                    break;
                case (int)HashName.SHA512:
                    hasher = new SHA512Hasher();
                    break;
            }
            return hasher;
        }

        private static SymmetricCryptor CreateSymmetricCryptor(int crypto, string key, string iv)
        {
            SymmetricCryptor cryptor = null;
            switch (crypto)
            {
                case (int)SymmetricCryptoName.Aes:
                    cryptor = new AesCryptor(key, iv);
                    break;
                case (int)SymmetricCryptoName.DES:
                    cryptor = new DESCryptor(key, iv);
                    break;
                case (int)SymmetricCryptoName.RC2:
                    cryptor = new RC2Cryptor(key, iv);
                    break;
                case (int)SymmetricCryptoName.Rijndael:
                    cryptor = new RijndaelCryptor(key, iv);
                    break;
                case (int)SymmetricCryptoName.TripleDES:
                    cryptor = new TripleDESCryptor(key, iv);
                    break;
            }
            return cryptor;
        }

        private static SymmetricCryptor CreateSymmetricCryptor(int crypto)
        {
            SymmetricCryptor cryptor = null;
            switch (crypto)
            {
                case (int)SymmetricCryptoName.Aes:
                    cryptor = new AesCryptor();
                    break;
                case (int)SymmetricCryptoName.DES:
                    cryptor = new DESCryptor();
                    break;
                case (int)SymmetricCryptoName.RC2:
                    cryptor = new RC2Cryptor();
                    break;
                case (int)SymmetricCryptoName.Rijndael:
                    cryptor = new RijndaelCryptor();
                    break;
                case (int)SymmetricCryptoName.TripleDES:
                    cryptor = new TripleDESCryptor();
                    break;
            }
            return cryptor;
        }


    }
}
