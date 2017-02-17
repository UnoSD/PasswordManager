using System.Text;

namespace Paccia
{
    public static class StringExtensions
    {
        public static byte[] ToUtf8Bytes(this string value) => value.ToBytes(Encoding.UTF8);
        public static byte[] ToAsciiBytes(this string value) => value.ToBytes(Encoding.ASCII);
        public static byte[] ToBytes(this string value, Encoding encoding) => encoding.GetBytes(value);
    }
}