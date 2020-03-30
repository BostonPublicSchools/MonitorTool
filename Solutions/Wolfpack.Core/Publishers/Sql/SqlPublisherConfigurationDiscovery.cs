using Wolfpack.Core.Configuration;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Publishers.Sql
{
    public class SqlPublisherConfigurationDiscovery : PluginDiscoveryBase<SqlPublisherConfiguration, SqlPublisher> 
    {
        protected override SqlPublisherConfiguration GetConfiguration()
        {
            return new SqlPublisherConfiguration
            {
                ConnectionName = "Wolfpackv3",
                Enabled = true,
                FriendlyId = "SqlPublisher",
                UseAsRepository = true
            };
        }

        protected override void Configure(ConfigurationEntry entry)
        {
            entry.Name = "SqlPublisher";
            entry.Description = "Publishes notifications to a sql database. Can optionally register as Wolfpack repository.";
            entry.Tags.AddIfMissing(SpecialTags.PUBLISHER, "Database", "SqlServer", "Repository");
        }
    }
}