using System.Collections.Generic;
using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.Interfaces.Magnum;

namespace Wolfpack.Core.Filters
{
    public class EventTypeFilter : ResultFilterBase
    {
        public List<string> PublishTheseTypes { get; set; }
        public List<string> IgnoreTheseTypes { get; set; }

        protected override bool ShouldPublish(INotificationEventPublisher publisher, NotificationEvent message)
        {
            if (IgnoreTheseTypes != null && IgnoreTheseTypes.Count > 0)
            {
                // don't publish if match
                if (IgnoreTheseTypes.Exists(ignoreType => System.String.CompareOrdinal(ignoreType, message.EventType) == 0))
                    return false;
                // otherwise check the publish list next....
            } 
            
            if (PublishTheseTypes != null && PublishTheseTypes.Count > 0)
            {
                // publish only if match
                return PublishTheseTypes.Exists(publishType => System.String.CompareOrdinal(publishType, message.EventType) == 0);
            }

            // publish everything else...
            return true;
        }
    }
}