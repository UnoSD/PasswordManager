using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paccia;

namespace WebServer.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public partial class SecretsController : Controller
    {
        [HttpGet("{url}")]
        public async Task<IEnumerable<Secret>> GetAsync(string url)
        {
            var sourceUri = new Uri(WebUtility.UrlDecode(url));

            var secrets = await CreateTestRepository().LoadAsync();

            return secrets.Where(secret => secret.Description == sourceUri.Host)
                          .Concat(new [] { new Secret { Description = "Test" } });
        }

        [HttpPost]
        public async Task PostAsync(Secret secret)
        {
            var repository = CreateTestRepository();

            var secrets = await repository.LoadAsync();
            
            await repository.SaveAsync(secrets.Concat(new [] { secret }));
        }
    }
}
