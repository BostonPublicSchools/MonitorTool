using System;
using System.Data;
using Wolfpack.Core.Database.Oracle;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Checks
{
    public class OracleQueryCheckConfig : QueryCheckConfigBase
    {
        public string ConnectionString { get; set; }
    }

    public class OracleQueryCheck : QueryCheckBase<OracleQueryCheckConfig>
    {
        /// <summary>
        /// default ctor
        /// </summary>
        public OracleQueryCheck(OracleQueryCheckConfig config)
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

            using (var cmd = OracleAdhocCommand.UsingSmartConnection(_config.ConnectionString)
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
            return "Oracle Scalar Check";
        }

        protected override PluginDescriptor BuildIdentity()
        {
            return new PluginDescriptor
                       {
                           Description = "Oracle Scalar Check",
                           TypeId = new Guid("FF47074E-798B-4e29-B098-D306EC5B5666"),
                           Name = _baseConfig.FriendlyId
                       };
        }
    }
}