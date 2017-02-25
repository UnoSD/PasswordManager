using System.IO;
using System.Security;
using System.Threading.Tasks;

namespace Paccia
{
    public interface IEncryptor
    {
        Task ToEncryptedStreamAsync(Stream sourceStream, Stream destinationStream, SecureString passphrase, string salt);
    }
}