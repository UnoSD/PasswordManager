using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Paccia;

namespace WebServer.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class SecretsController : Controller
    {
        readonly Repository<Secret> _repository;
        readonly IMemoryCache _cache;

        public SecretsController(Repository<Secret> repository, IMemoryCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        [HttpGet("{url}")]
        public async Task<JsonResult> GetAsync(string url)
        {
            var sourceUri = new Uri(WebUtility.UrlDecode(url));

            var cacheRefreshed = false;

            var secrets = await _cache.GetOrCreateAsync("secrets", ce =>
            {
                cacheRefreshed = true;
                return _repository.LoadAsync();
            });

            var secret = FindSecret(secrets, sourceUri);

            if (secret == null && !cacheRefreshed)
            {
                secrets = await _repository.LoadAsync();

                _cache.Set("secrets", secrets);

                secret = FindSecret(secrets, sourceUri);
            }

            return new JsonResult(new
            {
                username = secret?.Key ?? $"Not found",
                password = secret?.Value ?? "NF"
            });
        }

        static KeyValuePair<string, string>? FindSecret(IReadOnlyCollection<Secret> secrets, Uri sourceUri) =>
            secrets.FirstOrDefault(s => s.Url == sourceUri.Host)?
                   .Secrets
                   .FirstOrDefault();

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
