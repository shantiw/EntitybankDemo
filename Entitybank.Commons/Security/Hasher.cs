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
        // overload
        public string Hash(string value)
        {
            return Hash(value, new byte[0]);
        }

        // overload
        public string Hash(string value, string salt)
        {
            return Hash(value, Encoding.UTF8.GetBytes(salt));
        }

        public abstract string Hash(string value, byte[] salt);

        protected string Hash(HashAlgorithm hashAlgorithm, string plain, byte[] salt)
        {
            byte[] plainBytes = Encoding.UTF8.GetBytes(plain);
            byte[] buffer;
            if (salt.Length == 0)
            {
                buffer = plainBytes;
            }
            else
            {
                buffer = new byte[salt.Length + plainBytes.Length];
                Buffer.BlockCopy(salt, 0, buffer, 0, salt.Length);
                Buffer.BlockCopy(plainBytes, 0, buffer, salt.Length, plainBytes.Length);
            }

            byte[] result = hashAlgorithm.ComputeHash(buffer);
            return HexConverter.GetHexString(result);
        }


    }
}
