using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Publishers.Sql
{
    public class SqlPublisherConfiguration : PluginConfigBase
    {
        public string ConnectionName { get; set; }
        public bool UseAsRepository { get; set; }
    }
}