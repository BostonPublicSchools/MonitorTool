using Wolfpack.Core.Checks;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Testing.Domains;

namespace Wolfpack.Tests.Checks
{
    public class WindowServiceStartupDomain : HealthCheckDomain
    {
        private readonly WindowsServiceStartupCheckConfig _config;

        public WindowServiceStartupDomain(WindowsServiceStartupCheckConfig config, params INotificationRequestFilter[] filters)
            : base(filters)
        {
            _config = config;
        }

        protected override IHealthCheckPlugin HealthCheck
        {
            get { return new WindowsServiceStartupCheck(_config); }
        }
    }
}