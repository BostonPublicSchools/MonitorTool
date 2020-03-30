using System;
using System.Diagnostics;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.Notification;

namespace Wolfpack.Core.Schedulers
{
    public class HealthCheckIntervalSchedulerConfig : IntervalSchedulerConfig
    {
        
    }

    [DebuggerDisplay("{Identity.Name}")]
    public class HealthCheckIntervalScheduler : IntervalSchedulerBase, IHealthCheckSchedulerPlugin
    {
        protected IHealthCheckPlugin HealthCheck;
        protected PluginDescriptor InternalIdentity;

        public HealthCheckIntervalScheduler(IHealthCheckPlugin check,
            HealthCheckIntervalSchedulerConfig config) 
            : base(config)
        {
            HealthCheck = check;
            InternalIdentity = new PluginDescriptor
                             {
                                 TypeId = check.Identity.TypeId,
                                 Description = check.Identity.Description,
                                 Name = check.Identity.Name
                             };
        }

        public bool Enabled { get; set; }

        public override PluginDescriptor Identity
        {
            get { return InternalIdentity; }
        }

        /// <summary>
        /// Initialises the HealthCheck plugin
        /// </summary>
        public override void Initialise()
        {
            HealthCheck.Initialise();
        }

        /// <summary>
        /// Executes the health check plugin
        /// </summary>
        protected override void Execute()
        {
            try
            {
                HealthCheck.Execute();
            }
            catch (Exception ex)
            {
                var incidentCorrelationId = Guid.NewGuid();
                var msg = string.Format("Wolfpack Component Failure. IncidentId:={0}; Name:={1}; Details:={2}",
                    incidentCorrelationId,
                    HealthCheck.Identity.Name,
                    ex);

                Logger.Error(msg);

                // Broadcast a failure message
                Messenger.Publish(NotificationRequestBuilder.AlwaysPublish(new HealthCheckData
                                          {                                              
                                              CriticalFailure = true,
                                              CriticalFailureDetails = new CriticalFailureDetails
                                                                           {
                                                                               Id = incidentCorrelationId
                                                                           },
                                              GeneratedOnUtc = DateTime.UtcNow,
                                              Identity = HealthCheck.Identity,
                                              Info = ex.Message
                                          }).Build());
            }            
        }
    }
}