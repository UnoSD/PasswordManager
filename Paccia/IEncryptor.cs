using System.IO;
using System.Threading.Tasks;

namespace Paccia
{
    public interface IEncryptor {
        Task ToEncryptedStreamAsync(Stream sourceStream, Stream destinationStream, string passphrase, string salt);
    }
}