using System.Text;

namespace Paccia
{
    public static class ByteExtensions
    {
        public static string ToString(this byte[] value, Encoding encoding) => encoding.GetString(value);
        public static string ToUtf8String(this byte[] value) => value.ToString(Encoding.UTF8);
        public static string ToUnicodeString(this byte[] value) => value.ToString(Encoding.Unicode);
    }
}