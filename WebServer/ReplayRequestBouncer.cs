using System;
using Microsoft.Extensions.Caching.Memory;

namespace WebServer
{
    class ReplayRequestBouncer
    {
        public static bool IsReplayRequest(IMemoryCache memoryCache, HmacAuthenticationOptions options, string nonce, string requestUnixTimeStamp)
        {
            var nonceInMemory = memoryCache.Get(nonce);

            if (nonceInMemory != null)
                return true;

            var requestTime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(requestUnixTimeStamp));
            var now = DateTimeOffset.UtcNow;

            if (now - requestTime > options.MaxRequestAge)
                return true;

            memoryCache.Set(nonce, default(bool), now + options.MaxRequestAge);

            return false;
        }
    }
}