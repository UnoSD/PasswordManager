using System.Collections.Generic;
using System.Security;

namespace Paccia
{
    public class Secret
    {
        public string Url { get; set; }
        public string Description { get; set; }
        public IDictionary<string, string> Fields { get; } = new Dictionary<string, string>();
        public IDictionary<string, SecureString> Secrets { get; } = new Dictionary<string, SecureString>();
    }
}