using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Paccia
{
    public class Configuration : IConfiguration
    {
        readonly IConfigurationDefaults _defaults;
        readonly Lazy<IConfigurationRoot> _configuration;

        public Configuration(IConfigurationDefaults defaults)
        {
            _defaults = defaults;
            _configuration = new Lazy<IConfigurationRoot>(() => new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddXmlFile("App.config", true).Build());
        }

        public Task<string> GetAsync(ConfigurationKey configurationKey) =>
            GetAsync(configurationKey, _defaults.Values.GetValueOrDefault(configurationKey));

        Task<string> GetAsync(ConfigurationKey configurationKey, string defaultValue) =>
            Task.FromResult(_configuration.Value.GetSection("configuration")[configurationKey.ToString()] ?? defaultValue);
    }
}