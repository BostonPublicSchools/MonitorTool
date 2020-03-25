using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Checks
{
    /// <summary>
    /// This provides a base class for Checks that register a config component with 
    /// the container and then Wolfpack runtime would use a convention to infer the 
    /// actual Health Check component to load.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class HealthCheckBase<T> : IHealthCheckPlugin
    {
        protected T _config;
        private PluginDescriptor _identity;

        protected HealthCheckBase(T config)
        {
            _config = config;
        }
        
        public Status Status { get; set; }

        public virtual void Initialise()
        {
            // do nothing
        }

        public PluginDescriptor Identity
        {
            get { return _identity ?? (_identity = BuildIdentity()); }
        }

        public abstract void Execute();

        protected abstract PluginDescriptor BuildIdentity();

        protected virtual void Publish(NotificationRequest request)
        {
            Messenger.Publish(request);
        }
    }
}