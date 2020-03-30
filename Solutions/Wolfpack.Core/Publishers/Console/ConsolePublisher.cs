using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.Interfaces.Magnum;

namespace Wolfpack.Core.Publishers.Console
{
    public class ConsolePublisher : PublisherBase, INotificationEventPublisher
    {
        private readonly ConsolePublisherConfiguration _config;

        public ConsolePublisher(ConsolePublisherConfiguration config)
        {
            _config = config;

            Enabled = config.Enabled;
            FriendlyId = config.FriendlyId;
        }

        public void Consume(NotificationEvent message)
        {
            var graph = Serialiser.ToJson(message);
            System.Console.WriteLine(graph);
        }
    }
}
