using System;
using System.Text;

namespace Paccia
{
    public static class StringExtensions
    {
        public static byte[] ToUtf8Bytes(this string value) => value.ToBytes(Encoding.UTF8);
        public static byte[] ToAsciiBytes(this string value) => value.ToBytes(Encoding.ASCII);
        public static byte[] ToUnicodeBytes(this string value) => value.ToBytes(Encoding.Unicode);
        public static byte[] ToBytes(this string value, Encoding encoding) => encoding.GetBytes(value);
        public static string ToBase64(this string value) => value.ToUtf8Bytes().ToBase64();

        public static bool IsNullOrEmpty(this string value) => string.IsNullOrEmpty(value);

        public static bool EqualsInsensitiveCase(this string left, string right) => 
            left?.Equals(right, StringComparison.OrdinalIgnoreCase) ?? right == null;
    }
}