using System.IO;
using System.Threading.Tasks;

namespace Paccia
{
    public interface IDecryptor {
        Task ToDecryptedStreamAsync(Stream sourceStream, Stream destinationStream, string passphrase, string salt);
    }
}