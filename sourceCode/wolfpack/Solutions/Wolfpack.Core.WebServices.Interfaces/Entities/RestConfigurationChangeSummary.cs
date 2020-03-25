using System;
using System.Collections.Generic;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.WebServices.Interfaces.Entities
{
    public class RestConfigurationChangeSummary
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Action { get; set; }
        public List<RestLink> Links { get; set; }

        public RestConfigurationChangeSummary(ConfigurationChangeRequest change)
        {
            Id = change.Id;
            Name = change.Entry.Name;
            Action = change.Action;            
            Links = new List<RestLink>();
        }
    }
}