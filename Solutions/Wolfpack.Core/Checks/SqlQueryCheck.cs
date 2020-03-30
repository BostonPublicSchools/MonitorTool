using System;
using System.Data;
using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.Database.SqlServer;

namespace Wolfpack.Core.Checks
{
    public class SqlQueryCheckConfig : QueryCheckConfigBase
    {
        public string ConnectionString { get; set; }
    }

    public class SqlQueryCheck : QueryCheckBase<SqlQueryCheckConfig>
    {
        /// <summary>
        /// default ctor
        /// </summary>
        public SqlQueryCheck(SqlQueryCheckConfig config)
            : base(config)
        {
        }

        protected override void ValidateConfig()
        {
            base.ValidateConfig();

            if (_config.Query.Contains(";"))
                throw new FormatException("Semi-colons are not accepted in Sql from-query statements");
        }

        protected override DataTable RunQuery(string query)
        {
            var data = new DataTable();

            using (var cmd = SqlServerAdhocCommand.UsingSmartConnection(_config.ConnectionString)
                .WithSql(query))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    data.Load(reader);
                }
            }

            return data;
        }

        protected override string DescribeNotification()
        {
            return "Sql Query Check";
        }

        protected override PluginDescriptor BuildIdentity()
        {
            return new PluginDescriptor
            {
                Description = "Sql Query Check",
                TypeId = new Guid("7BFF8D1C-93EB-4f66-8719-5E6DDDED1E97"),
                Name = _baseConfig.FriendlyId
            };
        }
    }
}