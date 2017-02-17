using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Paccia;

namespace Test
{
    [TestFixture]
    public class EncryptionTest
    {
        const string ToEncrypt = "something to encrypt";
        const string Passphrase = "test passphrase";
        const string Salt = "pepper-pepper";

        ShaAesEncryptor _encryptor;
        ShaAesDecryptor _decryptor;

        [OneTimeSetUp]
        public void Init()
        {
            var encryptorDecryptor = new AesEncryptorDecryptor();
            var headerGenerator = new ShaHeaderGenerator();

            _encryptor = new ShaAesEncryptor(encryptorDecryptor, headerGenerator);
            _decryptor = new ShaAesDecryptor(encryptorDecryptor, headerGenerator);
        }

        [Test]
        public void EncryptDecrypt()
        {
            var encrypted = ToEncrypt.EncryptString(Passphrase, Salt);

            var decrypted = encrypted.DecryptString(Passphrase, Salt);

            Assert.That(ToEncrypt, Is.EqualTo(decrypted));
        }

        [Test]
        public async Task EncryptDecryptStream()
        {
            using (var toEncryptStream = new MemoryStream(ToEncrypt.ToUtf8Bytes()))
            using (var encryptedStream = new MemoryStream())
            using (var decryptedStream = new MemoryStream())
            {
                await _encryptor.ToEncryptedStreamAsync(toEncryptStream, encryptedStream, Passphrase, Salt);

                encryptedStream.Position = 0;
                
                await _decryptor.ToDecryptedStreamAsync(encryptedStream, decryptedStream, Passphrase, Salt);
                
                var result = decryptedStream.ToArray().ToUtf8String();

                Assert.That(result, Is.EqualTo(ToEncrypt));
            }
        }

        [Test]
        public async Task DecryptStream()
        {
            var encrypted = ToEncrypt.EncryptString(Passphrase, Salt);

            using (var encryptedStream = new MemoryStream(encrypted.ToArray()))
            using (var decryptedStream = new MemoryStream())
            {
                await _decryptor.ToDecryptedStreamAsync(encryptedStream, decryptedStream, Passphrase, Salt);
                
                var decrypted = decryptedStream.ToArray().ToUtf8String();

                Assert.That(decrypted, Is.EqualTo(ToEncrypt));
            }
        }

        [Test]
        public async Task EncryptStream()
        {
            using (var toEncryptStream = new MemoryStream(ToEncrypt.ToUtf8Bytes()))
            using (var encryptedStream = new MemoryStream())
            {
                await _encryptor.ToEncryptedStreamAsync(toEncryptStream, encryptedStream, Passphrase, Salt);

                var decrypted = encryptedStream.ToArray().DecryptString(Passphrase, Salt);

                Assert.That(decrypted, Is.EqualTo(ToEncrypt));
            }
        }
    }
}