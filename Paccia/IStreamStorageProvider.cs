using System.IO;
using System.Threading.Tasks;

namespace Paccia
{
    public interface IStreamStorageProvider
    {
        Task<Stream> GetStorageStream(FileAccess access);
    }
}