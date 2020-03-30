using Wolfpack.Core.Configuration;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Publishers.Console
{
    public class ConsolePublisherConfigurationDiscovery : PluginDiscoveryBase<ConsolePublisherConfiguration, ConsolePublisher>
    {
        protected override ConsolePublisherConfiguration GetConfiguration()
        {
            return new ConsolePublisherConfiguration
                       {
                           Enabled = true,
                           FriendlyId = "ConsolePublisher"
                       };
        }

        protected override void Configure(ConfigurationEntry entry)
        {
            entry.Name = "ConsolePublisher";
            entry.Description = "Publishes a json representation to the console window (when not running as a windows service) - handy for debugging!";
            entry.Tags.AddIfMissing(SpecialTags.PUBLISHER, "Console");
        }
    }
}