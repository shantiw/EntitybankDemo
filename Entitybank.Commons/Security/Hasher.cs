using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using XData.Data.Helpers;

namespace XData.Data.Security
{
    public abstract class Hasher
    {
        public string Hash(string str)
        {
            return Hash(str, null);
        }

        public abstract string Hash(string str, string salt);

        protected string Hash(HashAlgorithm hashAlgorithm, string plain, string salt)
        {
            byte[] plainBytes = Encoding.UTF8.GetBytes(plain);
            byte[] buffer;
            if (string.IsNullOrEmpty(salt))
            {
                buffer = plainBytes;
            }
            else
            {
                byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
                buffer = new byte[saltBytes.Length + plainBytes.Length];

                Buffer.BlockCopy(saltBytes, 0, buffer, 0, saltBytes.Length);
                Buffer.BlockCopy(plainBytes, 0, buffer, saltBytes.Length, plainBytes.Length);
            }

            byte[] result = hashAlgorithm.ComputeHash(buffer);
            return HexConverter.GetHexString(result);
        }


    }
}
