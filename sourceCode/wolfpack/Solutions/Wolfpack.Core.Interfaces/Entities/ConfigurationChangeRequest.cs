using System;

namespace Wolfpack.Core.Interfaces.Entities
{
    public class ConfigurationChangeRequest
    {
        public Guid Id { get; set; }
        public string Action { get; set; }
        public ConfigurationEntry Entry { get; set; }

        public ConfigurationChangeRequest()
        {
            Id = Guid.NewGuid();
        }
    }
}