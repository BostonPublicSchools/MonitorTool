using Wolfpack.Core.Checks;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Testing.Domains;

namespace Wolfpack.Tests.Checks
{   
    public class WindowServiceStateDomain : HealthCheckDomain
    {
        private readonly WindowsServiceStateCheckConfig _config;

        public WindowServiceStateDomain(WindowsServiceStateCheckConfig config,
            params INotificationRequestFilter[] filters) : base(filters)
        {
            _config = config;
        }

        protected override IHealthCheckPlugin HealthCheck
        {
            get { return new WindowsServiceStateCheck(_config); }
        }
    }
}