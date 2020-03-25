using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.WebUI.Interfaces.Entities
{
    public class SendNotificationModel
    {
        public SendNotificationModel(NotificationEvent notification, string[] apiKeys)
        {
            Notification = notification;
            ApiKeys = Serialiser.ToJson(apiKeys, false);
            Json = Serialiser.ToJson(notification, false);
        }

        public NotificationEvent Notification { get; private set; }
        public string ApiKeys { get; set; }
        public string Json { get; private set; }
    }
}