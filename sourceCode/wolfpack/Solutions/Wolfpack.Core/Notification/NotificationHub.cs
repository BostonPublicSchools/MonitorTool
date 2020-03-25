using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Notification
{
    public static class NotificationHub
    {
        private static INotificationHub _instance;

        public static void Initialise()
        {
            Container.RegisterAll<INotificationRequestFilter>();

            if (!Container.IsRegistered<INotificationHub>())
            {
                Logger.Info("Registering default NotificationHub");
                Container.RegisterAsSingleton<INotificationHub>(typeof(DefaultNotificationHub));
            }

            Initialise(Container.Resolve<INotificationHub>());
        }

        public static void Initialise(INotificationHub instance)
        {
            _instance = instance;
            _instance.Initialise();
        }

        public static NotificationEvent ConvertRequestToEvent(NotificationRequest request)
        {
            return _instance.ConvertRequestToEvent(request);
        }
    }
}