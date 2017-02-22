using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Paccia
{
    public class Repository<T>
    {
        readonly ISerializer<IEnumerable<T>> _serializer;
        readonly Lazy<Task<string>> _storageFilePath;

        internal Repository(IConfiguration configuration, ISerializer<IEnumerable<T>> serializer, ConfigurationKey filePathConfigurationKey)
        {
            _serializer = serializer;
            _storageFilePath  = new Lazy<Task<string>>(() => configuration.GetAsync(filePathConfigurationKey));
        }

        public async Task<IReadOnlyCollection<T>> LoadAsync()
        {
            var filePath = await _storageFilePath;

            var fileInfo = new FileInfo(filePath);

            if (!fileInfo.Exists)
            {
                Logger.Log("File not found");

                return new T[0];
            }
            
            using (var fileStream = fileInfo.OpenRead())
            using (var memoryStream = new MemoryStream(new byte[fileStream.Length]))
            {
                await fileStream.CopyToAsync(memoryStream).ConfigureAwait(false);

                memoryStream.Position = 0;

                var entities = await _serializer.DeserializeAsync(memoryStream).ConfigureAwait(false);

                return entities.ToArray();
            }
        }
        
        public async Task SaveAsync(IEnumerable<T> entities)
        {
            var filePath = await _storageFilePath;

            // The serializer is a wrapper around sync code so using a memory stream first then
            // copying to file to make it non-blocking on IO operation on file.
            using (var memoryStream = new MemoryStream())
            using (var fileStream = new FileInfo(filePath).OpenWrite())
            {
                await _serializer.SerializeAsync(memoryStream, entities).ConfigureAwait(false);

                memoryStream.Position = 0;
                   
                await memoryStream.CopyToAsync(fileStream).ConfigureAwait(false);
            }
        }
    }
}