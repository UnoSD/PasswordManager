using System.Security;
using NUnit.Framework;
using Paccia;

namespace Test
{
    [TestFixture]
    public class EncryptionEncodingHashingTest
    {
        const string TestString = "test string";
        static byte[] _testStringUnicodeBytes;
        static SecureString _testStringSecure;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _testStringUnicodeBytes = TestString.ToUnicodeBytes();
            _testStringSecure = TestString.ToSecureString();
        }
        
        [Test]
        public void ToSha512Base64()
        {
            var secureStringHash = _testStringSecure.ToUnicodeSha512Base64();

            var stringHash = _testStringUnicodeBytes.ToSha512Base64();

            Assert.That(secureStringHash, Is.EqualTo(stringHash));
        }

        [Test]
        public void EnumerableStream()
        {
            var stream = new EnumerableStream<byte>(_testStringSecure.ToLazyUnicodeBytes(), b => new [] { b });

            var buffer = new byte[22];

            var offset = 0;

            while (stream.Read(buffer, offset++, 1) != 0) { }
            
            Assert.That(buffer, Is.EquivalentTo(_testStringUnicodeBytes));
        }

        [Test]
        public void ToLazyUnicodeBytes()
        {
            var bytes = _testStringSecure.ToLazyUnicodeBytes();

            Assert.That(bytes, Is.EquivalentTo(_testStringUnicodeBytes));
        }

        [Test]
        public void ToUnicodeBytes()
        {
            var bytes = _testStringSecure.ToUnicodeBytes();
            
            Assert.That(bytes, Is.EquivalentTo(_testStringUnicodeBytes));
        }

        [Test]
        public void ToUnicodeString()
        {
            var bytes = _testStringSecure.ToUnicodeBytes();

            var value = bytes.ToUnicodeString();

            Assert.That(value, Is.EquivalentTo(TestString));
        }
    }
}