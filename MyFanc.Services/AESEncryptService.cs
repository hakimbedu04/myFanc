using MyFanc.Contracts.Services;
using System.Security.Cryptography;

namespace MyFanc.Services
{
    public class AESEncryptService : IAESEncryptService
    {
        private readonly ITokenConfiguration _tokenConfiguration;
        private const int DerivationIterations = 10000;
        private const int saltOrIvLength = 16;
        private const int NumberOfPseudoRandomKey = 32;
        public AESEncryptService(ITokenConfiguration tokenConfiguration) {
            _tokenConfiguration = tokenConfiguration;
        }
        public string DecryptString(string cipherTextCombinedString)
        {
            byte[] cipherTextCombined = Convert.FromBase64String(cipherTextCombinedString);
            byte[] salt = new byte[saltOrIvLength];
            byte[] iv = new byte[saltOrIvLength];
            byte[] cipherText = new byte[cipherTextCombined.Length - saltOrIvLength - saltOrIvLength];

            Array.Copy(cipherTextCombined, 0, salt, 0, salt.Length);
            Array.Copy(cipherTextCombined, salt.Length, iv, 0, iv.Length);
            Array.Copy(cipherTextCombined, salt.Length + iv.Length, cipherText, 0, cipherText.Length);

            byte[] key = DeriveKey(_tokenConfiguration.Key, salt);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

        public string EncryptString(string text)
        {
            byte[] salt = GenerateCryptographicallyRandomBytes();
            byte[] iv = GenerateCryptographicallyRandomBytes();
            byte[] key = DeriveKey(_tokenConfiguration.Key, salt);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(text);
                        }
                    }

                    byte[] cipherText = msEncrypt.ToArray();
                    byte[] combined = new byte[salt.Length + iv.Length + cipherText.Length];

                    Array.Copy(salt, 0, combined, 0, salt.Length);
                    Array.Copy(iv, 0, combined, salt.Length, iv.Length);
                    Array.Copy(cipherText, 0, combined, salt.Length + iv.Length, cipherText.Length);

                    return Convert.ToBase64String(combined);
                }
            }
        }

        private static byte[] GenerateCryptographicallyRandomBytes()
        {
            byte[] randomBytes = new byte[saltOrIvLength];
            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }

        private static byte[] DeriveKey(string password, byte[] salt)
        {
            using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, DerivationIterations, HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(NumberOfPseudoRandomKey); 
            }
        }
    }
}
