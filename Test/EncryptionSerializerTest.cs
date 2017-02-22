using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Paccia;

namespace Test
{
    [TestFixture]
    public class EncryptionSerializerTest
    {
        public async Task SerializeDeserialize()
        {
            const string toEncryptAndSerialize = "something to encrypt and serialize";

            var encryptorDecryptor = new AesEncryptorDecryptor();
            var headerGenerator = new ShaHeaderGenerator();
            var serializer = new EncryptionSerializer<string>(new BinarySerializer<string>(), new ShaAesEncryptor(encryptorDecryptor, headerGenerator), new ShaAesDecryptor(encryptorDecryptor, headerGenerator), "sample passphrase", "sample salt");

            using (var memoryStream = new MemoryStream())
            {
                await serializer.SerializeAsync(memoryStream, toEncryptAndSerialize);

                memoryStream.Position = 0;

                var actual = await serializer.DeserializeAsync(memoryStream);

                Assert.That(actual, Is.EqualTo(toEncryptAndSerialize));
            }
        }
    }
}