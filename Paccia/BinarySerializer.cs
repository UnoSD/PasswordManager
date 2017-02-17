using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace Paccia
{
    class BinarySerializer<T> : ISerializer<T>
    {
        public Task<T> DeserializeAsync(Stream sourceStream) =>
            Task.FromResult((T)new BinaryFormatter().Deserialize(sourceStream));

        public Task SerializeAsync(Stream destinationStream, T data)
        {
            new BinaryFormatter().Serialize(destinationStream, data);

            return Task.CompletedTask;
        }
    }
}