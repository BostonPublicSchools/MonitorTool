using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Notification.Filters.Request
{
    public abstract class NotificationRequestFilterBase : INotificationRequestFilter
    {
        public abstract string Mode { get; }
        public abstract bool Execute(NotificationRequest request);

        protected string GetKey(NotificationRequest request)
        {
            return (request.DataKeyGenerator ?? DefaultKeyGenerator).Invoke(request.Notification);
        }

        protected virtual string DefaultKeyGenerator(INotificationEventCore notification)
        {
            return notification.CheckId.ToLower();
        }
    }
}