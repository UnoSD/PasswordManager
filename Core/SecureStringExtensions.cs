using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;

namespace Paccia
{
    public static class SecureStringExtensions
    {
        public static string ToClearString(this SecureString value)
        {
            var bstr = SecureStringMarshal.SecureStringToCoTaskMemUnicode(value);

            try
            {
                return Marshal.PtrToStringBSTR(bstr);
            }
            finally
            {
                Marshal.ZeroFreeCoTaskMemUnicode(bstr);
            }
        }

        public static byte[] ToUnicodeBytes(this SecureString value)
        {
            var bstr = SecureStringMarshal.SecureStringToCoTaskMemUnicode(value);

            try
            {
                // TODO: This does not work in .NET Core, make sure the Lenght is reliable.
                //Marshal.ReadInt32(bstr, -4);
                var length = value.Length * 2;

                var bytes = new byte[length];

                // Let's pin it to prevent the GC from moving it in memory before we wipe it out.
                // TODO: I presume it's useless if we then free it later without clearing the content.
                // TODO: See the solution below.
                var bytesPin = GCHandle.Alloc(bytes, GCHandleType.Pinned);

                try
                {
                    Marshal.Copy(bstr, bytes, 0, length);

                    return bytes;
                }
                finally
                {
                    // TODO: The caller should zero the array then free the GC.
                    // TODO: Make it an IDisposable/high order function so the user can use
                    // TODO: it then we handle the clean up.
                    //for (var i = 0; i < bytes.Length; i++)
                    //    bytes[i] = 0;

                    bytesPin.Free();
                }

            }
            finally
            {
                // I presume Marshal.FreeBSTR(bstr) would just free the memory without wiping, check.
                Marshal.ZeroFreeCoTaskMemUnicode(bstr);
            }
        }

        // TODO: Move to test project, should not be used outside testing.
        internal static SecureString ToSecureString(this string value)
        {
            var secureString = new SecureString();

            foreach (var character in value)
                secureString.AppendChar(character);

            return secureString;
        }

        public static string ToUnicodeSha512Base64(this SecureString value) =>
            value.ToLazyUnicodeBytes().ToStream(b => new[] { b }).ToSha512Base64();

        public static string ToSha512Base64(this byte[] value)
        {
            using (var memoryStream = new MemoryStream(value))
                return memoryStream.ToSha512Base64();
        }

        public static string ToSha512Base64(this Stream stream)
        {
            using (var shaCalculator = SHA512.Create())
                return Convert.ToBase64String(shaCalculator.ComputeHash(stream));
        }

        public static IEnumerable<byte> ToLazyUnicodeBytes(this SecureString value)
        {
            var bstr = SecureStringMarshal.SecureStringToCoTaskMemUnicode(value);

            try
            {
                // TODO: This does not work in .NET Core, make sure the Lenght is reliable.
                //Marshal.ReadInt32(bstr, -4);
                var length = value.Length * 2;

                for (var index = 0; index < length; index++)
                    yield return Marshal.ReadByte(bstr, index);
            }
            finally
            {
                Marshal.ZeroFreeCoTaskMemUnicode(bstr);
            }
        }
    }
}