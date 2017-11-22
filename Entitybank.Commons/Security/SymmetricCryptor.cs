using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using XData.Data.Helpers;

namespace XData.Data.Security
{
    public abstract class SymmetricCryptor
    {
        public byte[] Key { get; private set; } = null;
        public byte[] IV { get; private set; } = null;

        protected SymmetricAlgorithm Algorithm { get; private set; }

        public SymmetricCryptor()
        {
            Algorithm = CreateSymmetricAlgorithm();
            Key = Algorithm.Key;
            IV = Algorithm.IV;
        }

        public SymmetricCryptor(string base64StringKey, string base64StringIV)
        {
            Key = Convert.FromBase64String(base64StringKey);
            IV = Convert.FromBase64String(base64StringIV);
            Algorithm = CreateSymmetricAlgorithm();
            Algorithm.Key = Key;
            Algorithm.IV = IV;
        }

        public SymmetricCryptor(byte[] key, byte[] iv)
        {
            Key = key;
            IV = iv;
            Algorithm = CreateSymmetricAlgorithm();
            Algorithm.Key = Key;
            Algorithm.IV = IV;
        }

        public virtual string Encrypt(string value)
        {
            return Encrypt(Algorithm, value);
        }

        public virtual string Decrypt(string encryptedString)
        {
            string text = Decrypt(Algorithm, encryptedString);
            text = text.TrimEnd('\0');
            return text;
        }

        protected abstract SymmetricAlgorithm CreateSymmetricAlgorithm();

        protected static string Encrypt(SymmetricAlgorithm algorithm, string plain)
        {
            byte[] plainBytes = Encoding.UTF8.GetBytes(plain);

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, algorithm.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(plainBytes, 0, plainBytes.Length);
                }
                byte[] cipherBytes = ms.ToArray();
                return HexConverter.GetHexString(cipherBytes);
            }
        }

        protected static string Decrypt(SymmetricAlgorithm algorithm, string encrypted)
        {
            byte[] cipherBytes = HexConverter.GetBytes(encrypted);

            using (MemoryStream ms = new MemoryStream(cipherBytes))
            {
                using (CryptoStream cs = new CryptoStream(ms, algorithm.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    byte[] plainBytes = new byte[cipherBytes.Length];
                    cs.Read(plainBytes, 0, cipherBytes.Length);
                    return Encoding.UTF8.GetString(plainBytes);
                }
            }
        }


    }
}
