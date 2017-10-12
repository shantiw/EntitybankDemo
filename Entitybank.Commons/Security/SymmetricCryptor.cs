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

        public SymmetricCryptor()
        {
        }

        public SymmetricCryptor(byte[] key, byte[] iv)
        {
            Key = key;
            IV = iv;
        }

        public virtual string Encrypt(string str)
        {
            SymmetricAlgorithm algorithm = GetSymmetricAlgorithm();
            return Encrypt(algorithm, str);
        }

        public virtual string Decrypt(string encryptedString)
        {
            SymmetricAlgorithm algorithm = GetSymmetricAlgorithm();
            string text = Decrypt(algorithm, encryptedString);
            text = text.TrimEnd('\0');
            return text;
        }

        protected virtual SymmetricAlgorithm GetSymmetricAlgorithm()
        {
            SymmetricAlgorithm algorithm = CreateSymmetricAlgorithm();
            if (Key == null)
            {
                Key = algorithm.Key;
            }
            else
            {
                algorithm.Key = Key;
            }
            if (IV == null)
            {
                IV = algorithm.IV;
            }
            else
            {
                algorithm.IV = IV;
            }
            return algorithm;
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

            using (MemoryStream memoryStream = new MemoryStream(cipherBytes))
            {
                using (CryptoStream cs = new CryptoStream(memoryStream,
                    algorithm.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    byte[] plainBytes = new byte[cipherBytes.Length];
                    cs.Read(plainBytes, 0, cipherBytes.Length);
                    return Encoding.UTF8.GetString(plainBytes);
                }
            }
        }


    }
}
