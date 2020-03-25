using Wolfpack.Core.Configuration;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Database.SQLite
{
    public class SQLitePublisherConfigurationDiscovery : PluginDiscoveryBase<SQLiteConfiguration, SQLitePublisher>
    {
        protected override SQLiteConfiguration GetConfiguration()
        {
            return new SQLiteConfiguration
                       {
                           Enabled = true,
                           FriendlyId = "CHANGEME!",
                           ConnectionString = "Wolfpack"
                       };
        }

        protected override void Configure(ConfigurationEntry entry)
        {
            entry.Name = "SQLitePublisher";
            entry.Description = "Publishes Notification events to a SQLite database";
            entry.Tags.AddIfMissing(SpecialTags.Publisher, "SQLite");
        }
    }
}