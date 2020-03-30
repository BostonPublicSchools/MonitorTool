using System;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.Notification;

namespace Wolfpack.Core.Checks
{
    /// <summary>
    /// Use this as a base for a Health Check component that is registered directly with the 
    /// container (as opposed to one that is has its config component registered). Registering
    /// the Health Checks directly with the container is the recommended integration route.
    /// </summary>
    public abstract class HealthCheckBaseEx : IHealthCheckPluginEx
    {
        private readonly PluginDescriptor _identity;

        public Status Status { get; set; }
        public bool Enabled { get; set; }
        public string NotificationMode { get; set; }

        protected HealthCheckBaseEx(string friendlyId,
            bool enabled,
            string notificationMode,
            Guid typeId,
            string description, 
            params object[] args)
        {
            Enabled = enabled;
            NotificationMode = notificationMode;

            _identity = new PluginDescriptor
                            {
                                Name = friendlyId,
                                Description = string.Format(description, args),
                                TypeId = typeId
                            };
        }

        public virtual void Initialise()
        {
            // do nothing
        }

        public PluginDescriptor Identity
        {
            get { return _identity; }
        }

        public abstract void Execute();

        protected virtual void Publish(HealthCheckData message)
        {
            Messenger.Publish(NotificationRequestBuilder.For(NotificationMode, message).Build());
        }
    }
}