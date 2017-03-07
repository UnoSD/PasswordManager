using System;
using System.Collections.Generic;
using System.IO;

namespace Paccia
{
    class PacciaHardcodedConfigurationDefaults : IConfigurationDefaults
    {
        public IReadOnlyDictionary<ConfigurationKey, string> Values => new Dictionary<ConfigurationKey, string>
        {
            [ConfigurationKey.SecretsFilePath] = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PacciaSecrets")
        };
    }
}