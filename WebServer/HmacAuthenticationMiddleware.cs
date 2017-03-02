using System;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WebServer
{
    public class HmacAuthenticationMiddleware : AuthenticationMiddleware<HmacAuthenticationOptions>
    {
        readonly IMemoryCache _memoryCache;

        public HmacAuthenticationMiddleware
        (
            RequestDelegate next,
            IOptions<HmacAuthenticationOptions> options,
            ILoggerFactory loggerFactory,
            UrlEncoder encoder,
            IMemoryCache memoryCache
        ) : base(next, options, loggerFactory, encoder)
        {
            if(memoryCache == null)
                throw new ArgumentNullException();

            _memoryCache = memoryCache;
        }

        protected override AuthenticationHandler<HmacAuthenticationOptions> CreateHandler() => 
            new HmacAuthenticationHandler(_memoryCache);
    }
}