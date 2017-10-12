using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace XData.Data.Security
{
    public enum SymmetricCryptoName { Aes, DES, RC2, Rijndael, TripleDES }

    public class DESCryptor : SymmetricCryptor
    {
        public DESCryptor() : base()
        {
        }

        public DESCryptor(byte[] key, byte[] iv) : base(key, iv)
        {
        }

        protected override SymmetricAlgorithm CreateSymmetricAlgorithm()
        {
            // Base64
            // KEY: "f9Sepo7BOho="
            // IV:  "ghQ7ysLG+6I="

            // Mode CBC; Padding PKCS7; Key	byte[8]; IV byte[8]
            SymmetricAlgorithm algorithm = new DESCryptoServiceProvider();
            return algorithm;
        }
    }

    public class RC2Cryptor : SymmetricCryptor
    {
        public RC2Cryptor() : base()
        {
        }

        public RC2Cryptor(byte[] key, byte[] iv) : base(key, iv)
        {
        }

        protected override SymmetricAlgorithm CreateSymmetricAlgorithm()
        {
            // Base64
            // KEY: "NepZXNMaXxzGUNgKJMDOEg=="
            // IV:  "yRTJkXjVW48="

            // Mode CBC; Padding PKCS7; Key	byte[16]; IV byte[8]
            SymmetricAlgorithm algorithm = new RC2CryptoServiceProvider();
            return algorithm;
        }
    }

    public class AesCryptor : SymmetricCryptor
    {
        public AesCryptor() : base()
        {
        }

        public AesCryptor(byte[] key, byte[] iv) : base(key, iv)
        {
        }

        protected override SymmetricAlgorithm CreateSymmetricAlgorithm()
        {
            // Base64 
            // KEY: "dJANDdEBc2iELLIEkrCJFI0NnuAnzAzh6lFZ5z1wios="
            // IV:  "zk+FAPsuuaKYP7+bYcohRw=="

            // Mode CBC; Padding PKCS7; Key	byte[32]; IV byte[16]
            SymmetricAlgorithm algorithm = new AesCryptoServiceProvider();
            return algorithm;
        }
    }

    public class RijndaelCryptor : SymmetricCryptor
    {
        public RijndaelCryptor() : base()
        {
        }

        public RijndaelCryptor(byte[] key, byte[] iv) : base(key, iv)
        {
        }

        protected override SymmetricAlgorithm CreateSymmetricAlgorithm()
        {
            // Base64
            // KEY: "PhvHK1fFhBEgxYOnXqXYvX7cQ2IlelYZYoe9v470/E4="
            // IV:  "mKuf3dEWIvS1R7dfMgIBsw=="

            // Mode CBC; Padding PKCS7; Key	byte[32]; IV byte[16]
            SymmetricAlgorithm algorithm = new RijndaelManaged();
            return algorithm;
        }
    }

    public class TripleDESCryptor : SymmetricCryptor
    {
        public TripleDESCryptor() : base()
        {
        }

        public TripleDESCryptor(byte[] key, byte[] iv) : base(key, iv)
        {
        }

        protected override SymmetricAlgorithm CreateSymmetricAlgorithm()
        {
            // Base64
            // KEY: "OQ5DCUDpZ7D/EDY94Totrb/jL9bEPaeC";
            // IV:  "H3an/vIh8j0=";

            // Mode CBC; Padding PKCS7; Key	byte[24]; IV byte[8]
            SymmetricAlgorithm algorithm = new TripleDESCryptoServiceProvider();
            return algorithm;
        }
    }

}
