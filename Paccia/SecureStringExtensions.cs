using System.Runtime.InteropServices;
using System.Security;

namespace Paccia
{
    public static class SecureStringExtensions
    {
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
    }
}