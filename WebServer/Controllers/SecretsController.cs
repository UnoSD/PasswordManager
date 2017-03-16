using System;
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
        public async Task<JsonResult> GetAsync(string url)
        {
            var sourceUri = new Uri(WebUtility.UrlDecode(url));

            var secrets = await CreateTestRepository().LoadAsync();

            // At some point will return the whole secret (or selected fields/secrets
            // and we will map which field/password goes to which page control.

            // Introduce property for Secret instead of First(): MainSecretKey.
            var secret = secrets.FirstOrDefault(s => s.Url == sourceUri.Host)
                               ?.Secrets
                                .FirstOrDefault();

            return new JsonResult(new
            {
                username = secret?.Key ?? $"ServerUserFor {sourceUri}",
                password = secret?.Value ?? "ServerPassword"
            });
        }

        [HttpPut("{url}")]
        public async Task PutAsync([ModelBinder(BinderType = typeof(SecretModelBinder))]Secret secret, string url)
        {
            var repository = CreateTestRepository();

            var secrets = await repository.LoadAsync();

            var sourceUri = new Uri(WebUtility.UrlDecode(url));

            secret.Url = sourceUri.Host;

            var existing = secrets.Where(s => s.Url == sourceUri.Host).ToArray();

            if (existing.Any())
                secrets = secrets.Except(existing).ToArray();

            await repository.SaveAsync(secrets.Concat(new[] { secret }));
        }
    }
}
