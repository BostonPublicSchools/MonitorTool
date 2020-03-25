using Wolfpack.Core.Configuration;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Database.SqlServer
{
    public class SqlServerPublisherConfigurationDiscovery : PluginDiscoveryBase<SqlServerConfiguration, SqlNotificationEventPublisher>
    {
        protected override SqlServerConfiguration GetConfiguration()
        {
            return new SqlServerConfiguration
                       {
                           Enabled = true,
                           FriendlyId = "CHANGEME!",
                           ConnectionString = "Wolfpack",
                           SchemaName = "dbo"
                       };
        }

        protected override void Configure(ConfigurationEntry entry)
        {
            entry.Name = "SqlServerPublisherBase";
            entry.Description = "Publishes Notification events to a SqlServer compatible database";
            entry.Tags.AddIfMissing(SpecialTags.PUBLISHER, "SqlServer");
        }
    }
}