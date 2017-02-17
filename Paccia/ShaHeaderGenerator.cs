using System.Security.Cryptography;

namespace Paccia
{
    public class ShaHeaderGenerator
    {
        public byte[] GetHashedHeader(string passphrase, string salt)
        {
            var saltedPassphrase = (passphrase + salt).ToAsciiBytes();

            using (var shaCalculator = new SHA512Managed())
                return shaCalculator.ComputeHash(saltedPassphrase);
        }
    }
}