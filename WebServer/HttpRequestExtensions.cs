using Microsoft.AspNetCore.Http;

namespace WebServer
{
    public static class HttpRequestExtensions
    {
        public static string GetAbsoluteUri(this HttpRequest request) =>
            $"{request.Scheme}://{request.Host.ToUriComponent()}{request.PathBase.ToUriComponent()}{request.Path.ToUriComponent()}{request.QueryString.ToUriComponent()}";
    }
}