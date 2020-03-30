using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.WebServices.Interfaces.Entities
{
    public class RestConfigurationChangeRequest
    {
        public string Action { get; set; }
        public RestCatalogueEntry Entry { get; set; }

        public ConfigurationChangeRequest ToChangeRequest()
        {
            return new ConfigurationChangeRequest
                             {
                                 Action = Action,
                                 Entry = Entry.ToEntry()
                             };
        }
    }
}