using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Database.SQLite
{
    public class SQLiteConfiguration : PluginConfigBase
    {
        public string ConnectionString { get; set; }
    }
}