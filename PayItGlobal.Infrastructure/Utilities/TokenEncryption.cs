using System;
using System.Security.Cryptography;
using System.Text;

namespace PayItGlobal.Infrastructure.Utilities
{
    public static class TokenEncryption
    {
        //TODO: Change this key to a secure value from kv secret
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("SUaEc7bGDG9BE8ZDBn+NRVHH0ACx/WHe");

        static TokenEncryption()
        {
            if (Key.Length != 32)
            {
                throw new ArgumentException("Key must be 32 bytes (256 bits) for AES-256 encryption.");
            }
        }

        public static string GenerateSecureToken()
        {
            var tokenData = new byte[32];
            RandomNumberGenerator.Fill(tokenData);
            return Convert.ToBase64String(tokenData);
        }

        public static (string EncryptedToken, string Iv) Encrypt(string plainText)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Key;
                aes.GenerateIV();
                var iv = Convert.ToBase64String(aes.IV);

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var ms = new System.IO.MemoryStream())
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var sw = new StreamWriter(cs))
                {
                    sw.Write(plainText);
                    sw.Close();
                    var encryptedToken = Convert.ToBase64String(ms.ToArray());
                    return (encryptedToken, iv);
                }
            }
        }

        public static string Decrypt(string encryptedText, string iv)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = Convert.FromBase64String(iv);

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var ms = new System.IO.MemoryStream(Convert.FromBase64String(encryptedText)))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        public static string Hash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
