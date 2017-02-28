using System.IO;
using System.Security;
using System.Threading.Tasks;

namespace Paccia
{
    public interface IDecryptor
    {
        Task ToDecryptedStreamAsync(Stream sourceStream, Stream destinationStream, SecureString passphrase, string salt);
    }
}