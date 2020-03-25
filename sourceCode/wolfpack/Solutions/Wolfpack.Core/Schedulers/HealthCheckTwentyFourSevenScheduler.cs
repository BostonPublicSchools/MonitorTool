using System.Diagnostics;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;
using System.Linq;

namespace Wolfpack.Core.Schedulers
{
    public class HealthCheckTwentyFourSevenSchedulerConfig : TwentyFourSevenTimerConfig
    {
        public int IntervalInSeconds { get; set; }

        public HealthCheckTwentyFourSevenSchedulerConfig()
        {
            IntervalInSeconds = 60;
        }
    }

    [DebuggerDisplay("{Identity.Name}")]
    public class HealthCheckTwentyFourSevenScheduler : HealthCheckIntervalScheduler
    {
        protected TwentyFourSevenTimer Timer;

        public HealthCheckTwentyFourSevenScheduler(IHealthCheckPlugin check,
            HealthCheckTwentyFourSevenSchedulerConfig config) 
            : base(check, new HealthCheckIntervalSchedulerConfig
                              {
                                  IntervalInSeconds = config.IntervalInSeconds
                              })
        {
            InternalIdentity = new PluginDescriptor
                             {
                                 TypeId = check.Identity.TypeId,
                                 Description = check.Identity.Description,
                                 Name = check.Identity.Name
                             };

            Timer = new TwentyFourSevenTimer(config);
        }

        /// <summary>
        /// Executes the health check plugin
        /// </summary>
        protected override void Execute()
        {
            if (!Timer.Triggered().Any())
                return;

            base.Execute();
        }
    }
}