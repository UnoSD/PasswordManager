using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Paccia;

namespace WebServer.Controllers
{
    [Route("[controller]")]
    public class SecretsController : Controller
    {
        [HttpGet("{url}")]
        public async Task<object> GetAsync(string url)
        {
            var sourceUri = new Uri(WebUtility.UrlDecode(url));

            return await GetSecretByUri(sourceUri.Host);
        }

        static async Task<IEnumerable<Secret>> GetSecretByUri(string host)
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

            var repository = new Repository<Secret>(encryptionSerializer, userFileStorageProvider);

            await AddSamples(host, repository);

            return (await repository.LoadAsync()).Where(secret => secret.Description == host);
        }

        static async Task AddSamples(string host, Repository<Secret> repository)
        {
            var secrets = new[]
                          {
                              new Secret {Description = host},
                              new Secret {Description = "http://www.google.it"}
                          };
            await repository.SaveAsync(secrets);
        }
    }
}
