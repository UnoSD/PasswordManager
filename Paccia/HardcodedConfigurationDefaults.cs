using System.Collections.Generic;

namespace Paccia
{
    public class HardcodedConfigurationDefaults : IConfigurationDefaults
    {
        public IReadOnlyDictionary<ConfigurationKey, string> Values => new Dictionary<ConfigurationKey, string>
        {
            [ConfigurationKey.SecretsFilePath] = "secrets"
        };
    }
}