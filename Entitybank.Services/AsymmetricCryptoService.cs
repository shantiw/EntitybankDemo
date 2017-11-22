using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XData.Data.Security;

namespace XData.Data.Services
{
    public class AsymmetricCryptoService
    {
        protected static RSACryptor RSACryptor = new RSACryptor();

        public string GetPublicKey()
        {
            return RSACryptor.PublicKey;
        }

        public string Decrypt(string base64String)
        {
            return RSACryptor.Decrypt(base64String, RSACryptor.Key);
        }


    }
}
