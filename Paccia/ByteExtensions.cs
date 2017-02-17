using System.Text;

namespace Paccia
{
    public static class ByteExtensions
    {
        public static string ToUtf8String(this byte[] value) => Encoding.UTF8.GetString(value);
    }
}