using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Paccia;

namespace WebServer
{
    /// <summary>
    /// A valid header will be the following:
    /// Authorization: Hmac AppID:Base64HmacSha256Hash(req):PerRequestUniqueID:Timestamp
    /// </summary>
    class HmacAuthenticationHandler : AuthenticationHandler<HmacAuthenticationOptions>
    {
        readonly IMemoryCache _memoryCache;
        readonly Lazy<byte[]> _secretKeyBytes;
        readonly Lazy<AuthenticationTicket> _ticket;

        public HmacAuthenticationHandler(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _secretKeyBytes = new Lazy<byte[]>(() => Convert.FromBase64String(Options.SecretKey));
            _ticket = new Lazy<AuthenticationTicket>(() => AuthenticationTicketFactory.CreateTicket(Options));
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var authorization = Request.Headers["authorization"].ToString();
            
            if (authorization.IsNullOrEmpty())
                return AuthenticateResult.Skip();

            var valid = await HmacRequestValidator.ValidateAsync(Request, authorization, Options, _memoryCache, _secretKeyBytes)
                                                  .ConfigureAwait(false);

            return valid ?
                   AuthenticateResult.Success(_ticket.Value) :
                   AuthenticateResult.Fail("Authentication failed.");
        }
    }
}