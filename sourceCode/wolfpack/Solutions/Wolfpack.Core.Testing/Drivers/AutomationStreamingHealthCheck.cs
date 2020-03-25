using System;
using System.Threading;
using Wolfpack.Core.Checks;
using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.Notification;
using Wolfpack.Core.Testing.Domains;
using System.Linq;

namespace Wolfpack.Core.Testing.Drivers
{
    public class AutomationStreamingHealthCheck : ThresholdCheckBase<ThresholdCheckDomainConfig>
    {
        public AutomationStreamingHealthCheck(ThresholdCheckDomainConfig config)
            : base(config)
        {
        }

        public override void Execute()
        {
            for (var i = 0; i < _config.Cycles; i++)
            {
                _config.Values.ToList().ForEach(rv =>
                                            {
                                                Publish(NotificationRequestBuilder.For(_config.NotificationMode,
                                                    HealthCheckData.For(Identity, "Fake Threshold Result")
                                                    .Succeeded()
                                                    .ResultCountIs(rv))
                                                    .Build());

                                                Thread.Sleep(_config.Interval);
                                            });

            }
        }

        protected override PluginDescriptor BuildIdentity()
        {
            return new PluginDescriptor
                       {
                           Description = "Test Automation HealthCheck",
                           Name = _config.CheckName ?? "TestAutomationHealthCheck",
                           TypeId = new Guid("17FFDF11-88A9-4095-A43C-111F2F7EC3AB")
                       };
        }
    }
}