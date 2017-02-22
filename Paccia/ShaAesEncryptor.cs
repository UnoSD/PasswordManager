using System.IO;
using System.Threading.Tasks;

namespace Paccia
{
    public class ShaAesEncryptor : IEncryptor
    {
        readonly AesEncryptorDecryptor _encryptorDecryptor;
        readonly ShaHeaderGenerator _headerGenerator;

        public ShaAesEncryptor(AesEncryptorDecryptor encryptorDecryptor, ShaHeaderGenerator headerGenerator)
        {
            _encryptorDecryptor = encryptorDecryptor;
            _headerGenerator = headerGenerator;
        }

        public async Task ToEncryptedStreamAsync(Stream sourceStream, Stream destinationStream, string passphrase, string salt)
        {
            var headerSalt = _headerGenerator.GetHashedHeader(passphrase, salt);

            await destinationStream.WriteAsync(headerSalt, 0, headerSalt.Length).ConfigureAwait(false);

            await _encryptorDecryptor.EncryptDecrtyptStreamAsync(passphrase, headerSalt, destinationStream, sourceStream, managed => managed.CreateEncryptor())
                                     .ConfigureAwait(false);
        }
    }
}