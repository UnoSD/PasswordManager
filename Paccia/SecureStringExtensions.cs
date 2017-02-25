using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Paccia
{
    public static class SecureStringExtensions
    {
#if MONO
        [System.Runtime.Versioning.ResourceExposure(System.Runtime.Versioning.ResourceScope.None)]
#else
        [DllImport("oleaut32.dll")]
#endif
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        internal static extern uint SysStringLen(IntPtr bstr);

        public static string ToClearString(this SecureString value)
        {
            var bstr = Marshal.SecureStringToBSTR(value);

            try
            {
                return Marshal.PtrToStringBSTR(bstr);
            }
            finally
            {
                Marshal.FreeBSTR(bstr);
            }
        }

        public static string ToClearStringManual(this SecureString value) => 
            Encoding.Unicode.GetString(value.ToBytes());

        public static byte[] ToBytes(this SecureString value)
        {
            var bstr = Marshal.SecureStringToBSTR(value);

            var len = SysStringLen(bstr);

            var bytes = new byte[len * 2];

            for (var i = 0; i < len * 2; i++)
                bytes[i] = Marshal.ReadByte(bstr, i);
            
            try
            {
                return bytes;
            }
            finally
            {
                Marshal.FreeBSTR(bstr);
            }
        }

        public static SecureString ToSecureString(this string value)
        {
            var secureString = new SecureString();

            foreach (var character in value)
                secureString.AppendChar(character);

            return secureString;
        }
    }
}