using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Testing.Domains
{
    public class HealthCheckDomainConfig : PluginConfigBase, ISupportNotificationMode
    {
        public string CheckName { get; set; }

        public string NotificationMode { get; set; }

        public static HealthCheckDomainConfig NotificationModeIs(string mode)
        {
            return new HealthCheckDomainConfig
            {
                NotificationMode = mode
            };
        }
    }

    public abstract class HealthCheckDomain : SystemDomainBase
    {
        protected HealthCheckDomain()
        {
        }

        protected HealthCheckDomain(params INotificationRequestFilter[] filters)
        {
            Agent.Run(filters);
        }

        protected abstract IHealthCheckPlugin HealthCheck { get; }

        public virtual void TheHealthCheckIsInvoked()
        {
            SafeExecute(() =>
                            {
                                var sut = HealthCheck;
                                sut.Initialise();
                                sut.Execute();
                            });
        }
    }
}