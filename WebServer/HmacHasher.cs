using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace WebServer
{
    class HmacHasher
    {
        public static string GetDataToHash(HttpRequest request, string appId, string nonce, string requestTimeStamp, string content)
        {
            // Workaround for Kestrel bug:
            // It is unescaping only %3A to : (not %2F to /.)
            request.Path = request.Path.Value.Replace(":", "%3A");

            var url = request.GetDisplayUrl();
            
            var encodedUri = WebUtility.UrlEncode(url);
            
            return $"{appId}{request.Method}{encodedUri}{requestTimeStamp}{nonce}{content}";
        }
    }
}