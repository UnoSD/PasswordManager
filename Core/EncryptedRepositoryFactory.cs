using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security;

namespace Paccia
{
    public class EncryptedRepositoryFactory<T>
    {
        readonly ConcurrentDictionary<string, Repository<T>> _repositories = new ConcurrentDictionary<string, Repository<T>>();
        readonly EncryptionSerializersFactory<IEnumerable<T>> _encryptionSerializersFactory;
        readonly IConfiguration _configuration;
        readonly Logger _logger;

        public EncryptedRepositoryFactory(EncryptionSerializersFactory<IEnumerable<T>> encryptionSerializersFactory, IConfiguration configuration, Logger logger)
        {
            _encryptionSerializersFactory = encryptionSerializersFactory;
            _configuration = configuration;
            _logger = logger;
        }

        internal Repository<T> GetRepository(SecureString passphrase, string salt, ConfigurationKey filePathConfigurationKey) => 
            _repositories.GetOrAdd(passphrase.ToUnicodeSha512Base64() + salt, _ => CreateRepository(passphrase, salt, filePathConfigurationKey));

        Repository<T> CreateRepository(SecureString passphrase, string salt, ConfigurationKey filePathConfigurationKey)
        {
            var serializer = _encryptionSerializersFactory.GetSerializer(passphrase, salt);

            return new Repository<T>(serializer, new UserFileStorageProvider(_configuration, filePathConfigurationKey, _logger));
        }
    }
}