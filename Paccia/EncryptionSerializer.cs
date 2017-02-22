using System.IO;
using System.Threading.Tasks;

namespace Paccia
{
    class EncryptionSerializer<T> : ISerializer<T>
    {
        readonly ISerializer<T> _serializer;
        readonly IEncryptor _encryptor;
        readonly IDecryptor _decryptor;
        readonly string _passphrase;
        readonly string _salt;

        // To match the ISerializer<T> interface we must take passphrase and salt as parameter
        // restricting the usages of this class' instances.
        public EncryptionSerializer(ISerializer<T> serializer, IEncryptor encryptor, IDecryptor decryptor, string passphrase, string salt)
        {
            _serializer = serializer;
            _encryptor = encryptor;
            _decryptor = decryptor;
            _passphrase = passphrase;
            _salt = salt;
        }

        public async Task<T> DeserializeAsync(Stream sourceStream)
        {
            // Implement a BridgeStream to automatically and lazily read from one when writing to another.
            // Try BufferedStream.
            using (var memoryStream = new MemoryStream())
            {
                await _decryptor.ToDecryptedStreamAsync(sourceStream, memoryStream, _passphrase, _salt);

                memoryStream.Position = 0;

                return await _serializer.DeserializeAsync(memoryStream);
            }
        }

        public async Task SerializeAsync(Stream destinationStream, T data)
        {
            using (var memoryStream = new MemoryStream())
            {
                await _serializer.SerializeAsync(memoryStream, data);

                memoryStream.Position = 0;

                await _encryptor.ToEncryptedStreamAsync(memoryStream, destinationStream, _passphrase, _salt);
            }
        }
    }
}