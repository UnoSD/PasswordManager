using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace WebServer
{
    class HmacHasher
    {
        public static string GetDataToHash(HttpRequest request, string appId, string nonce, string requestTimeStamp, string content)
        {
            var path = request.HttpContext
                              .Features
                              .Get<IHttpRequestFeature>()
                              .RawTarget;

            return $"{appId}{request.Method}{path}{requestTimeStamp}{nonce}{content}";
        }
    }
}