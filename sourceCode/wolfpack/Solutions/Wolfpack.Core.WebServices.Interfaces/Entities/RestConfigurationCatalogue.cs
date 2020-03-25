using System;
using System.Collections.Generic;

namespace Wolfpack.Core.WebServices.Interfaces.Entities
{
    public class RestConfigurationCatalogue
    {
        public Guid InstanceId { get; set; }
        public List<RestConfigurationChangeSummary> Pending { get; set; }
        public List<RestCatalogueEntry> Items { get; set; }
        public List<RestLink> Links { get; set; }
    }
}