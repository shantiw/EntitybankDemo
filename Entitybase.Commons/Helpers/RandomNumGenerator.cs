using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace XData.Data.Helpers
{
    public class RandomNumGenerator
    {
        public const int DEFAULT_SALT_LENGTH = 16;

        public byte[] Generate()
        {
            return Generate(DEFAULT_SALT_LENGTH);
        }

        public byte[] Generate(int length)
        {
            byte[] buffer = new byte[length];
            (new RNGCryptoServiceProvider()).GetBytes(buffer);
            return buffer;
        }
    }
}
