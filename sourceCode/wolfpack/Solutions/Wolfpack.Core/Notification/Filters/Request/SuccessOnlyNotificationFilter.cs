using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Notification.Filters.Request
{
    public class SuccessOnlyNotificationFilter : NotificationRequestFilterBase
    {
        public const string FilterName = "SuccessOnly";
        
        public override string Mode { get { return FilterName; } }

        public override bool Execute(NotificationRequest request)
        {
            return (request.Notification.Result.GetValueOrDefault(false));
        }
    }
}