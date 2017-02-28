using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Paccia
{
    class JsonSerializer<T> : ISerializer<T>
    {
        readonly Lazy<JsonSerializer> _jsonSerializer = new Lazy<JsonSerializer>();

        public Task<T> DeserializeAsync(Stream sourceStream) =>
            Task.FromResult(_jsonSerializer.Value.Deserialize<T>(new JsonTextReader(new StreamReader(sourceStream))));
        
        public Task SerializeAsync(Stream destinationStream, T data)
        {
            var streamWriter = new StreamWriter(destinationStream);

            _jsonSerializer.Value.Serialize(streamWriter, data);
            
            streamWriter.Flush();

            return Task.CompletedTask;
        }
    }
}