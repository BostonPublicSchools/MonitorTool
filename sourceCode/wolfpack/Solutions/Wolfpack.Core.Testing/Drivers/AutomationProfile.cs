using System;
using System.Collections.Generic;
using System.Threading;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.Interfaces.Magnum;
using Wolfpack.Core.Notification;

namespace Wolfpack.Core.Testing.Drivers
{
    /// <summary>
    /// This role profile allows you to programmatically add implementations to the IoC
    /// container which are later used by the role component
    /// </summary>
    public class AutomationProfile
    {
        private IRolePlugin _role;

        private readonly List<INotificationRequestFilter> _notificationFilters; 

        private readonly AutomationLoader<IStartupPlugin> _startupLoader;
        private readonly AutomationLoader<INotificationEventPublisher> _publisherLoader;
        private readonly AutomationLoader<IHealthCheckSchedulerPlugin> _checkLoader;
        private readonly AutomationLoader<IActivityPlugin> _activityLoader;
        private readonly List<Action<AutomationProfile>> _customPluginActions;
        private readonly ManualResetEventSlim _waitGate;

        public IList<NotificationEvent> NotificationEvents { get; private set; }
        public IList<NotificationRequest> NotificationRequests { get; private set; }       
        
        private AutomationProfile()
        {
            _waitGate = new ManualResetEventSlim(false);
            _startupLoader = new AutomationLoader<IStartupPlugin>();
            _activityLoader = new AutomationLoader<IActivityPlugin>();
            _checkLoader = new AutomationLoader<IHealthCheckSchedulerPlugin>();
            _publisherLoader = new AutomationLoader<INotificationEventPublisher>();
            _notificationFilters = new List<INotificationRequestFilter>();
            _customPluginActions = new List<Action<AutomationProfile>>();

            NotificationRequests = new List<NotificationRequest>();
            NotificationEvents = new List<NotificationEvent>();
        }

        public static AutomationProfile Configure()
        {
           return new AutomationProfile(); 
        }

        public AutomationProfile Run(Action<AutomationProfile> action)
        {
            _customPluginActions.Add(action);
            return this;
        }

        public AutomationProfile Run(IStartupPlugin plugin)
        {
            _startupLoader.Add(plugin);
            return this;
        }

        public AutomationProfile Run(IHealthCheckSchedulerPlugin plugin)
        {
            _checkLoader.Add(plugin);
            return this;
        }

        public AutomationProfile Run(IActivityPlugin plugin)
        {
            _activityLoader.Add(plugin);
            return this;
        }

        public AutomationProfile Run(INotificationEventPublisher plugin)
        {
            _publisherLoader.Add(plugin);
            return this;
        }

        public AutomationProfile Run(params INotificationRequestFilter[] plugin)
        {
            _notificationFilters.AddRange(plugin);
            return this;
        }

        public IRolePlugin Start()
        {
            var agentConfig = new AgentConfiguration
                                  {
                                      AgentId = "TestAgent"
                                  };

            Container.Initialise();
            Messenger.Initialise(new MagnumMessenger());
            Messenger.InterceptBefore<NotificationEvent>(msg => NotificationEvents.Add(msg));
            Messenger.InterceptBefore<NotificationRequest>(msg => NotificationRequests.Add(msg));
            NotificationHub.Initialise(new DefaultNotificationHub(agentConfig, _notificationFilters));

            RunStartupPlugins();

            _customPluginActions.ForEach(a => a(this));

            _role = new Agent.Roles.Agent(agentConfig,
                                           _publisherLoader,
                                           _checkLoader,
                                           _activityLoader);
            _role.Start();
            return _role;
        }

        public void Stop()
        {
            _role.Stop();
        }

        protected virtual void RunStartupPlugins()
        {
            IStartupPlugin[] plugins;
            _startupLoader.Load(out plugins);
        }

        public void WaitUntil(string description, int seconds, Func<bool> check)
        {
            for (var i = 0; i < seconds; i++)
            {
                _waitGate.Wait(1000);
                if (check())
                    return;
            }

            throw new TimeoutException(string.Format("Timed out after {0}s waiting for event '{1}'", seconds, description));
        }
    }
}