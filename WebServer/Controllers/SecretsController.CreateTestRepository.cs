using System.Collections.Generic;
using Paccia;

namespace WebServer.Controllers
{
    public partial class SecretsController
    {
        static Repository<Secret> CreateTestRepository()
        {
            var aesEncryptorDecryptor = new AesEncryptorDecryptor();
            var shaHeaderGenerator = new ShaHeaderGenerator();
            var dataSerializer = new JsonSerializer<IEnumerable<Secret>>();
            var shaAesEncryptor = new ShaAesEncryptor(aesEncryptorDecryptor, shaHeaderGenerator);
            var shaAesDecryptor = new ShaAesDecryptor(aesEncryptorDecryptor, shaHeaderGenerator);
            var hardcodedConfigurationDefaults = new HardcodedConfigurationDefaults();
            var configuration = new Configuration(hardcodedConfigurationDefaults);
            var logger = new Logger();

            var encryptionSerializersFactory = new EncryptionSerializersFactory<IEnumerable<Secret>>(dataSerializer, shaAesEncryptor, shaAesDecryptor);

            var encryptionSerializer = encryptionSerializersFactory.GetSerializer("pippo".ToSecureString(), "pepper");

            var userFileStorageProvider = new UserFileStorageProvider(configuration, ConfigurationKey.SecretsFilePath, logger);

            return new Repository<Secret>(encryptionSerializer, userFileStorageProvider);
        }
    }
}