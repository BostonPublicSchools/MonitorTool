using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Interfaces
{
    public interface ISupportConfigurationDiscovery
    {
        ConfigurationEntry GetConfigurationMetadata();
    }
}