using System;
using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.Interfaces.Magnum;

namespace Wolfpack.Core.Publishers
{
    public abstract class FilteredResultPublisherBase<T> : PublisherBase, INotificationEventPublisher
        where T: PluginConfigBase
    {
        private readonly string _matchFriendlyName;
        private readonly Func<NotificationEvent, bool> _filter;
        protected abstract void Publish(NotificationEvent message);

        protected T Config { get; private set; }

        protected FilteredResultPublisherBase(T config,
            string friendlyName)
        {
            Config = config;
            Enabled = config.Enabled;
            FriendlyId = config.FriendlyId;

            _matchFriendlyName = friendlyName;
            _filter = MatchOnFriendlyName;
        }

        protected FilteredResultPublisherBase(T config, Func<NotificationEvent, bool> filter)
        {
            Config = config;
            _filter = filter;
        }

        public void Consume(NotificationEvent message)
        {
            if (!_filter(message))
            {

                return;
            }

            Publish(message);
        }

        protected virtual bool MatchOnFriendlyName(NotificationEvent message)
        {
            return string.Equals(message.CheckId, _matchFriendlyName, StringComparison.OrdinalIgnoreCase);
        }
    }
}