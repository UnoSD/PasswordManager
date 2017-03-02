using System;
using System.Security.Cryptography;
using System.Text;

namespace Paccia
{
    public static class ByteExtensions
    {
        public static string ToString(this byte[] value, Encoding encoding) => encoding.GetString(value);
        public static string ToUtf8String(this byte[] value) => value.ToString(Encoding.UTF8);
        public static string ToUnicodeString(this byte[] value) => value.ToString(Encoding.Unicode);
        public static string ToBase64(this byte[] value) => Convert.ToBase64String(value);

        public static string ToBase64HmacSha256Hash(this byte[] buffer, byte[] key)
        {
            using (var hmac = new HMACSHA256(key))
                return hmac.ComputeHash(buffer).ToBase64();
        }

        public static string ToBase64Md5Hash(this byte[] buffer)
        {
            using (var md5 = MD5.Create())
                return md5.ComputeHash(buffer).ToBase64();
        }
    }
}