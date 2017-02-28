using System.IO;
using System.Threading.Tasks;

namespace Paccia
{
    public interface ISerializer<T>
    {
        Task<T> DeserializeAsync(Stream sourceStream);
        Task SerializeAsync(Stream destinationStream, T data);
    }
}