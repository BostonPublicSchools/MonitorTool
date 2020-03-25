using System.Collections.Generic;
using Wolfpack.Core.Interfaces.Entities;
using System.Linq;

namespace Wolfpack.Core.WebServices.Interfaces.Entities
{
    public class RestCatalogueEntry
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Tags { get; set; }
        public string InterfaceType { get; set; }
        public string ConcreteType { get; set; }
        public string Data { get; set; }
        public List<NameValuePair> RequiredProperties { get; set; }
        public List<RestLink> Links { get; set; }

        public RestCatalogueEntry()
        {
            Links = new List<RestLink>();
            RequiredProperties = new List<NameValuePair>();
        }

        public RestCatalogueEntry(ConfigurationEntry entry)
            : this()
        {
            Name = entry.Name;
            Description = entry.Description;
            Tags = entry.Tags;
            InterfaceType = entry.PluginType;
            ConcreteType = entry.ConfigurationType;
            Data = entry.Data;
            RequiredProperties = entry.RequiredProperties.Select(p => new NameValuePair(p)).ToList();
        }

        public ConfigurationEntry ToEntry()
        {
            return new ConfigurationEntry
                       {
                           ConfigurationType = ConcreteType,
                           Data = Data,
                           Description = Description,
                           PluginType = InterfaceType,
                           Name = Name,
                           Tags = Tags,
                           RequiredProperties = new Properties(RequiredProperties.ToDictionary(nvp => nvp.Name, nvp => nvp.Value))
                       };
        }
    }
}