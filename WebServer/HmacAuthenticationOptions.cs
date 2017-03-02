using System;
using Microsoft.AspNetCore.Builder;

namespace WebServer
{
    public class HmacAuthenticationOptions : AuthenticationOptions
    {
        public TimeSpan MaxRequestAge { get; set; } = TimeSpan.FromSeconds(300);

        public string AppId { get; set; }

        public string SecretKey { get; set; }

        public HmacAuthenticationOptions()
        {
            AuthenticationScheme = "Hmac";
            AutomaticChallenge = true;
        }
    }
}