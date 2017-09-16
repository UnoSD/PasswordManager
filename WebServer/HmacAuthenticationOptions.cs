using System;
using Microsoft.AspNetCore.Authentication;

namespace WebServer
{
    public class HmacAuthenticationOptions : AuthenticationSchemeOptions
    {
        public TimeSpan MaxRequestAge { get; set; }
        public string AppId { get; set; }
        public string SecretKey { get; set; }
    }
}