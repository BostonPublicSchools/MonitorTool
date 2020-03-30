using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.Interfaces.Magnum;

namespace Wolfpack.Core.Filters
{
    public class SuccessFilter : ResultFilterBase
    {
        public bool PublishSuccess { get; set; }
        public bool PublishFailure { get; set; }

        protected override bool ShouldPublish(INotificationEventPublisher publisher, NotificationEvent message)
        {
            if (PublishFailure && !message.Result.GetValueOrDefault(false))
                return true;
            if (PublishSuccess && message.Result.GetValueOrDefault(false))
                return true;
            return false;
        }
    }
}