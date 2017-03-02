using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Authentication;

namespace WebServer
{
    class AuthenticationTicketFactory
    {
        public static AuthenticationTicket CreateTicket(AuthenticationOptions options)
        {
            var principal = new ClaimsPrincipal(new ClaimsIdentity("HMAC"));
            var properties = new AuthenticationProperties();
            var scheme = options.AuthenticationScheme;

            return new AuthenticationTicket(principal, properties, scheme);
        }
    }
}