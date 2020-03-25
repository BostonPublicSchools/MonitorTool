using System;
using System.Linq;
using Wolfpack.Core;
using Wolfpack.Core.Hosts;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.Interfaces.Magnum;
using Wolfpack.Core.Notification;

namespace Wolfpack.Agent.Roles
{
    public class Agent : PluginHostBase, IRolePlugin
    {
        protected readonly ILoader<INotificationEventPublisher> PublisherLoader;
        protected readonly ILoader<IHealthCheckSchedulerPlugin> ChecksLoader;
        protected readonly ILoader<IActivityPlugin> ActivitiesLoader;

        private readonly AgentConfiguration _config;
        protected PluginDescriptor InternalIdentity;

        public Agent(AgentConfiguration config,
            ILoader<INotificationEventPublisher> publisherLoader,
            ILoader<IHealthCheckSchedulerPlugin> checksLoader,
            ILoader<IActivityPlugin> activitiesLoader)
        {
            _config = config;
            PublisherLoader = publisherLoader;
            ChecksLoader = checksLoader;
            ActivitiesLoader = activitiesLoader;

            InternalIdentity = new PluginDescriptor
                             {
                                 Description = "Agent description [TODO]",
                                 Name = "Agent",
                                 TypeId = new Guid("649D0AAC-3AA0-4457-B82D-F834EA324CFA")
                             };
        }

        public override PluginDescriptor Identity
        {
            get { return InternalIdentity; }
        }

        public override void Start()
        {
            // start the load process....
            var sessionInfo = new NotificationEventAgentStart
            {
                DiscoveryStarted = DateTime.UtcNow,
                AgentId = _config.AgentId,
                SiteId = _config.SiteId,
                Id = _config.InstanceId
            };

            INotificationEventPublisher[] publishers;
            PublisherLoader.Load(out publishers,
                                        p =>
                                            {
                                                if (!p.Status.IsHealthy())
                                                {
                                                    Logger.Debug(
                                                        "*** Notification Publisher '{0}' reporting 'unhealthy', disabling it ***",
                                                        p.FriendlyId);
                                                    return;
                                                }

                                                Messenger.Subscribe(p);
                                                Logger.Debug("Loaded Notification Publisher '{0}'",
                                                             p.GetType().Name);
                                            });
            
            // load activities...
            IActivityPlugin[] activities;
            if (ActivitiesLoader.Load(out activities))
                activities.ToList().ForEach(
                    a =>
                        {
                            if (!a.Status.IsHealthy())
                            {
                                Logger.Debug("*** Activity '{0}' reporting 'unhealthy', skipping it ***",
                                             a.Identity.Name);
                                return;
                            }

                            _plugins.Add(a);
                            Logger.Debug("Loaded Activity '{0}'", a.GetType().Name);
                        });

            // load health checks...
            IHealthCheckSchedulerPlugin[] healthChecks;
            ChecksLoader.Load(out healthChecks);
            healthChecks.ToList().ForEach(
                h =>
                    {
                        if (!h.Status.IsHealthy())
                        {
                            Logger.Debug("*** HealthCheck '{0}' reporting 'unhealthy', skipping it ***", h.Identity.Name);
                            return;
                        }

                        _plugins.Add(h);
                        Logger.Debug("Loaded HealthCheck '{0}'", h.Identity.Name);
                    });

            // extract check info, attach and publish it to a session message
            sessionInfo.DiscoveryCompleted = DateTime.UtcNow;

            // TODO - add support for publishers
            sessionInfo.Checks = (from healthCheck in healthChecks
                                  where healthCheck.Status.IsHealthy()
                                  select healthCheck.Identity).ToList();
            sessionInfo.UnhealthyChecks = (from healthCheck in healthChecks
                                  where !healthCheck.Status.IsHealthy()
                                  select healthCheck.Identity).ToList();
            sessionInfo.UnhealthyPublishers = (from healthCheck in healthChecks
                                  where !healthCheck.Status.IsHealthy()
                                  select healthCheck.Identity).ToList();
            sessionInfo.Activities = (from activity in activities
                                      where activity.Status.IsHealthy()
                                      select activity.Identity).ToList();
            sessionInfo.UnhealthyActivities = (from activity in activities
                                      where !activity.Status.IsHealthy()
                                      select activity.Identity).ToList();

            Messenger.Publish(NotificationRequestBuilder.AlwaysPublish(sessionInfo).Build());

            // finally start the checks & activities
            base.Start();
        }
    }
}