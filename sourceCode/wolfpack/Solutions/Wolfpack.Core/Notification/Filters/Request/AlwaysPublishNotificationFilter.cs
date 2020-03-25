using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Notification.Filters.Request
{
    public class AlwaysPublishNotificationFilter : NotificationRequestFilterBase
    {
        public const string FilterName = "AlwaysPublish";

        public override string Mode { get { return FilterName; }}

        public override bool Execute(NotificationRequest request)
        {
            return true;
        }
    }
}