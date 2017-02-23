using System.Collections.Concurrent;

namespace Paccia
{
    public class EncryptionSerializersFactory<T>
    {
        readonly ConcurrentDictionary<string, EncryptionSerializer<T>> _serializers = new ConcurrentDictionary<string, EncryptionSerializer<T>>();
        readonly ISerializer<T> _dataSerializer;
        readonly IEncryptor _encryptor;
        readonly IDecryptor _decryptor;

        public EncryptionSerializersFactory(ISerializer<T> dataSerializer, IEncryptor encryptor, IDecryptor decryptor)
        {
            _dataSerializer = dataSerializer;
            _encryptor = encryptor;
            _decryptor = decryptor;
        }

        internal EncryptionSerializer<T> GetSerializer(string passphrase, string salt) =>
            _serializers.GetOrAdd
                         (
                              passphrase + salt,
                              _ =>
                                new EncryptionSerializer<T>
                                (
                                    _dataSerializer,
                                    _encryptor,
                                    _decryptor,
                                    passphrase,
                                    salt
                                )
                         );
    }
}