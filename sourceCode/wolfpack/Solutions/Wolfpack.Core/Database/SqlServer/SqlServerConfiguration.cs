using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Database.SqlServer
{
    public class SqlServerConfiguration : PluginConfigBase
    {
        private string schemaName = "dbo";

        public string ConnectionString { get; set; }
        
        public string SchemaName
        {
            get { return schemaName; }
            set { schemaName = value; }
        }
    }
}