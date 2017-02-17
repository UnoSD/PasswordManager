using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Paccia
{
    public class ShaAesDecryptor
    {
        readonly AesEncryptorDecryptor _encryptorDecryptor;
        readonly ShaHeaderGenerator _headerGenerator;

        public ShaAesDecryptor(AesEncryptorDecryptor encryptorDecryptor, ShaHeaderGenerator headerGenerator)
        {
            _encryptorDecryptor = encryptorDecryptor;
            _headerGenerator = headerGenerator;
        }

        public async Task ToDecryptedStreamAsync(Stream sourceStream, Stream destinationStream, string passphrase, string salt)
        {
            var headerSalt = _headerGenerator.GetHashedHeader(passphrase, salt);

            var sourceHeaderSalt = new byte[64];

            await sourceStream.ReadAsync(sourceHeaderSalt, 0, sourceHeaderSalt.Length)
                              .ConfigureAwait(false);

            if (!headerSalt.SequenceEqual(sourceHeaderSalt))
                throw new InvalidOperationException("Wrong passphrase and/or salt or invalid source.");

            await _encryptorDecryptor.EncryptDecrtyptStreamAsync(passphrase, headerSalt, destinationStream, sourceStream, managed => managed.CreateDecryptor())
                                     .ConfigureAwait(false);
        }
    }
}