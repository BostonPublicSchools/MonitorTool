using System.Linq;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Repositories.Queries
{
    public class NotificationHasTagsQuery : INotificationRepositoryQuery
    {
        private readonly string[] _tags;

        public NotificationHasTagsQuery(params string[] tags)
        {
            _tags = tags;
        }

        public IQueryable<NotificationEvent> Filter(IQueryable<NotificationEvent> data)
        {
            return data.Where(msg => msg.Tags.ContainsAll(_tags));
        }
    }
}