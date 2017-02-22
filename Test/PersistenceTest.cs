using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Paccia;

namespace Test
{
    [TestFixture]
    public class PersistenceTest
    {
        [Test]
        public async Task Save()
        {
            var filePath = Path.GetTempFileName();

            var configuration = Substitute.For<IConfiguration>();

            configuration.GetAsync(ConfigurationKey.SecretsFilePath).Returns(filePath);

            var binarySerializer = new BinarySerializer<IEnumerable<Secret>>();

            var repository = new Repository<Secret>(configuration, binarySerializer, ConfigurationKey.SecretsFilePath);

            await repository.SaveAsync(new[]
            {
                new Secret { Description = "Primo" },
                new Secret { Description = "Secondo" },
                new Secret { Description = "Terzo" }
            });

            var readAllBytes = File.ReadAllBytes(filePath);

            File.Delete(filePath);

            Secret[] secrets;

            using (var memoryStream = new MemoryStream(readAllBytes))
                secrets = (await binarySerializer.DeserializeAsync(memoryStream)).ToArray();

            Assert.That(secrets.Length, Is.EqualTo(3));
        }
    }
}
