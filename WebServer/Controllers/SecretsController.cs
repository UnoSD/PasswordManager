using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
namespace WebServer.Controllers
{
    [Route("[controller]")]
    public class SecretsController : Controller
    {
        [HttpGet("{url}")]
        public string Get(string url)
        {
            var sourceUri = new Uri(WebUtility.UrlDecode(url));

            return $"Secret for host: {sourceUri.Host}";
        }
    }
}
