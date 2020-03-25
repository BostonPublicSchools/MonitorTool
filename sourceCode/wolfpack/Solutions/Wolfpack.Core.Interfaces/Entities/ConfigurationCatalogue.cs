using System;
using System.Collections.Generic;

namespace Wolfpack.Core.Interfaces.Entities
{
    public class ConfigurationCatalogue
    {
        public Guid InstanceId { get; set; }
        public IEnumerable<ConfigurationChangeRequest> Pending { get; set; }
        public IEnumerable<ConfigurationEntry> Items { get; set; }
    }
}