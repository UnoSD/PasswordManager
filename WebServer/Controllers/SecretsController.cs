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
    public class SecretsController : Controller
    {
        readonly Repository<Secret> _repository;

        public SecretsController(Repository<Secret> repository) => _repository = repository;

        [HttpGet("{url}")]
        public async Task<JsonResult> GetAsync(string url)
        {
            var sourceUri = new Uri(WebUtility.UrlDecode(url));

            var secrets = await _repository.LoadAsync();

            // At some point will return the whole secret (or selected fields/secrets
            // and we will map which field/password goes to which page control.

            // Introduce property for Secret instead of First(): MainSecretKey.
            var secret = secrets.FirstOrDefault(s => s.Url == sourceUri.Host)
                               ?.Secrets
                                .FirstOrDefault();

            return new JsonResult(new
            {
                username = secret?.Key ?? $"Not found",
                password = secret?.Value ?? "NF"
            });
        }

        [HttpPut("{url}")]
        public async Task PutAsync([ModelBinder(BinderType = typeof(SecretModelBinder))]Secret secret, string url)
        {
            var secrets = await _repository.LoadAsync();

            var sourceUri = new Uri(WebUtility.UrlDecode(url));

            secret.Url = sourceUri.Host;

            var existing = secrets.Where(s => s.Url == sourceUri.Host).ToArray();

            if (existing.Any())
                secrets = secrets.Except(existing).ToArray();

            await _repository.SaveAsync(secrets.Concat(new[] { secret }));
        }
    }
}
