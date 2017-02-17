using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Paccia
{
    public class AesEncryptorDecryptor
    {
        const int KeyIterations = 32768;

        public async Task EncryptDecrtyptStreamAsync(string passphrase, byte[] saltBytes, Stream destinationStream, Stream sourceStream, Func<AesManaged, ICryptoTransform> transform)
        {
            var passphraseBytes = passphrase.ToUtf8Bytes();

            var key = new Rfc2898DeriveBytes(passphraseBytes, saltBytes, KeyIterations);

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