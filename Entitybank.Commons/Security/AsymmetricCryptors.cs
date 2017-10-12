using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace XData.Data.Security
{
    public enum AsymmetricCryptoName { RSA }

    public class RSACryptor
    {
        public string PublicKey { get; private set; }
        public string Key { get; private set; }

        public RSACryptor()
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            Key = rsa.ToXmlString(true);
            PublicKey = rsa.ToXmlString(false);
        }

        public static string Encrypt(string str, string publicKey)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(str);
            byte[] bytes = Encrypt(buffer, publicKey);
            return Convert.ToBase64String(bytes);
        }

        public static string Decrypt(string base64String, string key)
        {
            byte[] encrypted = Convert.FromBase64String(base64String);
            byte[] data = Decrypt(encrypted, key);
            return Encoding.UTF8.GetString(data);
        }

        // encrypt with public key
        protected static byte[] Encrypt(byte[] data, string publicKey)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(publicKey);

            byte[] bytes = rsa.Encrypt(data, true);
            return bytes;
        }

        // decrypt with private key
        protected static byte[] Decrypt(byte[] encrypted, string key)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(key);

            byte[] data = rsa.Decrypt(encrypted, true);
            return data;
        }

    }
}
