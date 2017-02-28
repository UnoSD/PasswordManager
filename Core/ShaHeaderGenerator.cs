using System.Linq;
using System.Security;
using System.Security.Cryptography;

namespace Paccia
{
    public class ShaHeaderGenerator
    {
        public byte[] GetHashedHeader(SecureString passphrase, string salt)
        {
            var saltedPassphrase = passphrase.ToUnicodeBytes().Concat(salt.ToAsciiBytes()).ToArray();

            using (var shaCalculator = SHA512.Create())
                return shaCalculator.ComputeHash(saltedPassphrase);
        }
    }
}