using System;
using System.Collections.Generic;
using System.IO;
using Paccia;

namespace WebServer
{
    public class WebServerHardcodedConfigurationDefaults : IConfigurationDefaults
    {
        public IReadOnlyDictionary<ConfigurationKey, string> Values => new Dictionary<ConfigurationKey, string>
        {
            [ConfigurationKey.SecretsFilePath] = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PacciaSecrets")
        };
    }
}