using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace server.Helpers
{
    public class Encryption
    {
        private byte[] salt;

        public void GenerateSalt()
        {
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
        }

        public string Encrypt(string password)
        {
            GenerateSalt();
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            return Convert.ToBase64String(hashBytes);
        }

        public bool Auth(string content, string password)
        {
            byte[] hashBytes = Convert.FromBase64String(password);
            byte[] userSalt = new byte[16];
            Array.Copy(hashBytes, 0, userSalt, 0, 16);
            var pkdf2 = new Rfc2898DeriveBytes(content, userSalt, 10000);
            byte[] hash = pkdf2.GetBytes(20);
            bool ok = true;
            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                {
                    ok = false;
                }
            }
            if (ok)
            {
                return true;
            }
            return false;
        }

    }
}
