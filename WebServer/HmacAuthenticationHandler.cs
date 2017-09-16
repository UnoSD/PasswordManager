using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Paccia;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Text.Encodings.Web;

namespace WebServer
{
    /// <summary>
    /// A valid header will be the following:
    /// Authorization: Hmac AppID:Base64HmacSha256Hash(req):PerRequestUniqueID:Timestamp
    /// </summary>
    class HmacAuthenticationHandler : AuthenticationHandler<HmacAuthenticationOptions>
    {
        public const string SchemeName = "Hmac";

        readonly IMemoryCache _memoryCache;
        readonly Lazy<byte[]> _secretKeyBytes;
        readonly Lazy<AuthenticationTicket> _ticket;

        public HmacAuthenticationHandler(IMemoryCache memoryCache, IOptionsMonitor<HmacAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            _memoryCache = memoryCache;
            _secretKeyBytes = new Lazy<byte[]>(() => Convert.FromBase64String(Options.SecretKey));
            _ticket = new Lazy<AuthenticationTicket>(() =>
            {
                var principal = new ClaimsPrincipal(new ClaimsIdentity(SchemeName.ToUpper()));
                var properties = new AuthenticationProperties();

                return new AuthenticationTicket(principal, properties, SchemeName);
            });
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var authorization = Request.Headers["authorization"].ToString();
            
            if (authorization.IsNullOrEmpty())
                return AuthenticateResult.NoResult();

            var valid = await HmacRequestValidator.ValidateAsync(Request, authorization, Options, _memoryCache, _secretKeyBytes)
                                                  .ConfigureAwait(false);

            return valid ?
                   AuthenticateResult.Success(_ticket.Value) :
                   AuthenticateResult.Fail("Authentication failed.");
        }
    }
}