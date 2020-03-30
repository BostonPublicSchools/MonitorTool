using Wolfpack.Core.Configuration;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.WebServices.Strategies.Steps
{
    public class MessageStalenessCheckConfigDiscovery : PluginDiscoveryBase<MessageStalenessCheckConfig>
    {
        protected override MessageStalenessCheckConfig GetConfiguration()
        {
            return new MessageStalenessCheckConfig
                       {
                           MaxAgeInMinutes = 5
                       };
        }

        protected override void Configure(ConfigurationEntry entry)
        {
            entry.Description = string.Format("Provides configuration for the {0} component which can be executed when a message is received from a remote Agent instance", 
                                              typeof(MessageStalenessCheckStep).Name);
            entry.Tags.AddIfMissing(SpecialTags.ACTIVITY, "WebService", "Step", "Strategy");
        }
    }
}