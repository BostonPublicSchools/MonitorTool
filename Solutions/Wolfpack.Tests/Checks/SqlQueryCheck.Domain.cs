using Wolfpack.Core.Checks;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Testing.Domains;

namespace Wolfpack.Tests.Checks
{
    public class SqlQueryCheckDomain : HealthCheckDomain
    {
        private readonly SqlQueryCheckConfig _config;

        public SqlQueryCheckDomain(SqlQueryCheckConfig config)
        {
            _config = config;
        }

        protected override IHealthCheckPlugin HealthCheck
        {
            get { return new SqlQueryCheck(_config); }
        }
    }
}