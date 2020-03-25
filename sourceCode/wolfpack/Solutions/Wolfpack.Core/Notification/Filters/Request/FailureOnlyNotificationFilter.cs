using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Notification.Filters.Request
{
    public class FailureOnlyNotificationFilter : NotificationRequestFilterBase
    {
        public const string FilterName = "FailureOnly";

        public override string Mode { get { return FilterName; }}

        public override bool Execute(NotificationRequest request)
        {
            return (request.Notification.Result.GetValueOrDefault(true) == false);
        }
    }
}