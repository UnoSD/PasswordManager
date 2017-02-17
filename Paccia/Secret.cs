using System;
using System.Collections.Generic;

namespace Paccia
{
    [Serializable]
    public class Secret
    {
        public string Description { get; set; }
        public IDictionary<string, string> Fields { get; } = new Dictionary<string, string>();
        public IDictionary<string, string> Secrets { get; } = new Dictionary<string, string>();
    }
}