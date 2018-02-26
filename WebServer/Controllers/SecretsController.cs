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

            var allSecrets = await _cache.GetOrCreateAsync("secrets", ce =>
            {
                cacheRefreshed = true;
                return _repository.LoadAsync();
            });

            var matchingSecrets = GetSecrets(allSecrets, sourceUri);

            if (!matchingSecrets.Any() && !cacheRefreshed)
            {
                allSecrets = await _repository.LoadAsync();

                _cache.Set("secrets", allSecrets);

                matchingSecrets = GetSecrets(allSecrets, sourceUri);
            }

            return matchingSecrets.Count < 2 ?

            new JsonResult(new
            {
                username = matchingSecrets.SingleOrDefault().Key ?? $"Not found",
                password = matchingSecrets.SingleOrDefault().Value ?? "NF"
            }) : new JsonResult(new
            {
                keys = matchingSecrets.Keys
            });
        }

        static IDictionary<string, string> GetSecrets(IReadOnlyCollection<Secret> secrets, Uri sourceUri) =>
            secrets.Where(s => s.Url == sourceUri.Host)
                   .SelectMany(s => s.Secrets)
                   .ToDictionary(p => p.Key, p => p.Value);

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
