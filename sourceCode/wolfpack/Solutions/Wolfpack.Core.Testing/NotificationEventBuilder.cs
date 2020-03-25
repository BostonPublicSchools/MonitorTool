using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Testing
{
    public class NotificationEventBuilder
    {
        private NotificationEvent _notification;

        public static NotificationEventBuilder From(INotificationEventCore coreEvent)
        {
            return new NotificationEventBuilder
                       {
                           _notification = new NotificationEvent
                                               {
                                                   AgentId = coreEvent.AgentId,
                                                   CheckId = coreEvent.CheckId,
                                                   CriticalFailure = coreEvent.CriticalFailure,
                                                   CriticalFailureDetails = coreEvent.CriticalFailureDetails,
                                                   DayBucket = coreEvent.DayBucket,
                                                   EventType = coreEvent.EventType,
                                                   GeneratedOnUtc = coreEvent.GeneratedOnUtc,
                                                   HourBucket = coreEvent.HourBucket,
                                                   Id = coreEvent.Id,
                                                   Latitude = coreEvent.Latitude,
                                                   Longitude = coreEvent.Longitude,
                                                   Message = coreEvent.Message,
                                                   MinuteBucket = coreEvent.MinuteBucket,
                                                   Properties = coreEvent.Properties,
                                                   ReceivedOnUtc = coreEvent.ReceivedOnUtc,
                                                   Result = coreEvent.Result,
                                                   ResultCount = coreEvent.ResultCount,
                                                   DisplayUnit = coreEvent.DisplayUnit,
                                                   SiteId = coreEvent.SiteId,
                                                   Tags = coreEvent.Tags,
                                                   Version = coreEvent.Version,
                                                   Data = Serialiser.ToJson(coreEvent)
                                               }
                       };

        }

        public NotificationEvent Build()
        {
            return _notification;
        }
    }
}