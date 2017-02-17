using System.Collections.Generic;

namespace Paccia
{
    interface IConfigurationDefaults
    {
        IReadOnlyDictionary<ConfigurationKey, string> Values { get; }
    }
}