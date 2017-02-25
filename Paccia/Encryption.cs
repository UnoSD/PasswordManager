using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace Paccia
{
    public static class EncryptionExtensions
    {
        public static byte[] EncryptString(this string unencryptedData, SecureString passphrase, string salt)
        {
            var passphraseAndSaltSha = GetPassphraseAndSaltSha(passphrase, salt);

            using (var memoryStream = new MemoryStream())
            {
                var unencryptedDataBytes = Encoding.UTF8.GetBytes(unencryptedData);
                var passphraseBytes = passphrase.ToBytes();

                var strongSaltBytes = passphraseAndSaltSha.Concat("D4D7879B-0F23-4D61-B54E-83EF5DC699BB".ToUtf8Bytes()).ToArray();

                var key = new Rfc2898DeriveBytes(passphraseBytes, strongSaltBytes, 32768);

                using (Aes aes = new AesManaged())
                {
                    aes.KeySize = 256;
                    aes.Key = key.GetBytes(aes.KeySize / 8);
                    aes.IV = key.GetBytes(aes.BlockSize / 8);

                    using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        cryptoStream.Write(unencryptedDataBytes, 0, unencryptedDataBytes.Length);
                }

                var encryptedData = memoryStream.ToArray();

                return passphraseAndSaltSha.Concat(encryptedData).ToArray();
            }
        }

        public static string DecryptString(this byte[] encryptedData, SecureString passphrase, string salt)
        {
            var passphraseAndSaltSha = GetPassphraseAndSaltSha(passphrase, salt);

            if (!encryptedData.Take(64).SequenceEqual(passphraseAndSaltSha))
                throw new InvalidOperationException();

            var encryptedDataWithoutLeadingSha = encryptedData.Skip(64).ToArray();

            var passphraseBytes = passphrase.ToBytes();

            var strongSaltBytes = passphraseAndSaltSha.Concat("D4D7879B-0F23-4D61-B54E-83EF5DC699BB".ToUtf8Bytes()).ToArray();

            var key = new Rfc2898DeriveBytes(passphraseBytes, strongSaltBytes, 32768);

            using (var aes = new AesManaged())
            {
                aes.KeySize = 256;
                aes.Key = key.GetBytes(aes.KeySize / 8);
                aes.IV = key.GetBytes(aes.BlockSize / 8);

                using (var memoryStream = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                        cryptoStream.Write(encryptedDataWithoutLeadingSha, 0, encryptedDataWithoutLeadingSha.Length);

                    var clearBytes = memoryStream.ToArray();

                    return Encoding.UTF8.GetString(clearBytes);
                }
            }
        }

        static byte[] GetPassphraseAndSaltSha(SecureString passphrase, string salt)
        {
            var passphraseAndSalt = passphrase.ToBytes().Concat(salt.ToAsciiBytes()).ToArray();

            using (var shaCalculator = new SHA512Managed())
                return shaCalculator.ComputeHash(passphraseAndSalt);
        }
    }
}