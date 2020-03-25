using System.Linq;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Repositories.Queries
{
    public class NotificationByStateQuery : INotificationRepositoryQuery
    {
        private readonly int _state;

        public NotificationByStateQuery(MessageStateTypes state)
        {
            _state = (int)state;
        }

        public IQueryable<NotificationEvent> Filter(IQueryable<NotificationEvent> data)
        {            
            return data.Where(msg => ((int)msg.State) == _state);
        }
    }
}