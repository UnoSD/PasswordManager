using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Paccia
{
    public class Repository<T>
    {
        readonly ISerializer<IEnumerable<T>> _serializer;
        readonly IStreamStorageProvider _streamStorageProvider;

        public Repository(ISerializer<IEnumerable<T>> serializer, IStreamStorageProvider streamStorageProvider)
        {
            _serializer = serializer;
            _streamStorageProvider = streamStorageProvider;
        }

        public async Task<IReadOnlyCollection<T>> LoadAsync()
        {
            using (var fileStream = await _streamStorageProvider.GetStorageStream(FileAccess.Read).ConfigureAwait(false))
            {
                if(fileStream == Stream.Null)
                    return new T[0];

                using (var memoryStream = new MemoryStream(new byte[fileStream.Length]))
                {
                    await fileStream.CopyToAsync(memoryStream).ConfigureAwait(false);

                    memoryStream.Position = 0;

                    var entities = await _serializer.DeserializeAsync(memoryStream).ConfigureAwait(false);

                    return entities.ToArray();
                }
            }
        }

        public async Task SaveAsync(IEnumerable<T> entities)
        {
            // The serializer is a wrapper around sync code so using a memory stream first then
            // copying to file to make it non-blocking on IO operation on file.
            using (var memoryStream = new MemoryStream())
            using (var fileStream = await _streamStorageProvider.GetStorageStream(FileAccess.Write).ConfigureAwait(false))
            {
                await _serializer.SerializeAsync(memoryStream, entities).ConfigureAwait(false);

                memoryStream.Position = 0;

                await memoryStream.CopyToAsync(fileStream).ConfigureAwait(false);
            }
        }
    }
}