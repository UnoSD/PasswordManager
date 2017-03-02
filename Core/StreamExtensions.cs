using System.IO;
using System.Threading.Tasks;

namespace Paccia
{
    public static class StreamExtensions
    {
        public static async Task<MemoryStream> ToMemoryStreamAsync(this Stream stream)
        {
            var memoryStream = new MemoryStream();

            await stream.CopyToAsync(memoryStream);

            return memoryStream;
        }
    }
}