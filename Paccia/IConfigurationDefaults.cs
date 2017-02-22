using System.Collections.Generic;

namespace Paccia
{
    public interface IConfigurationDefaults
    {
        IReadOnlyDictionary<ConfigurationKey, string> Values { get; }
    }
}