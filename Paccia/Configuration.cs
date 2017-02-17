using System.Configuration;
using System.Threading.Tasks;

namespace Paccia
{
    class Configuration : IConfiguration
    {
        readonly IConfigurationDefaults _defaults;

        public Configuration(IConfigurationDefaults defaults)
        {
            _defaults = defaults;
        }

        public Task<string> GetAsync(ConfigurationKey configurationKey) =>
            GetAsync(configurationKey, _defaults.Values.GetValueOrDefault(configurationKey));

        static Task<string> GetAsync(ConfigurationKey configurationKey, string defaultValue) =>
            Task.FromResult(ConfigurationManager.AppSettings.Get(configurationKey.ToString()) ?? defaultValue);
    }
}