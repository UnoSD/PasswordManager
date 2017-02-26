using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Paccia
{
    public class AesEncryptorDecryptor
    {
        const int KeyIterations = 32768;

        public async Task EncryptDecrtyptStreamAsync(SecureString passphrase, byte[] saltBytes, Stream destinationStream, Stream sourceStream, Func<AesManaged, ICryptoTransform> transform)
        {
            var passphraseBytes = passphrase.ToUnicodeBytes();

            var strongSaltBytes = saltBytes.Concat("D4D7879B-0F23-4D61-B54E-83EF5DC699BB".ToUtf8Bytes()).ToArray();

            var key = new Rfc2898DeriveBytes(passphraseBytes, strongSaltBytes, KeyIterations);

            using (var aes = new AesManaged())
            {
                aes.KeySize = 256;
                aes.Key = key.GetBytes(aes.KeySize / 8);
                aes.IV = key.GetBytes(aes.BlockSize / 8);

                var cryptoStream = new CryptoStream(destinationStream, transform(aes), CryptoStreamMode.Write);

                await sourceStream.CopyToAsync(cryptoStream).ConfigureAwait(false);

                cryptoStream.FlushFinalBlock();

                // TODO: CHECK: Do we need to return cryptoStream to dispose it? Or it doesn't need to as it just disposes the internal stream?
            }
        }
    }
}