using System;
using System.Linq;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Interfaces
{
    public interface INotificationRepository
    {
        void Initialise();
        IQueryable<NotificationEvent> Filter(params INotificationRepositoryQuery[] filters);

        bool GetById(Guid id, out NotificationEvent notification);
        IQueryable<NotificationEvent> GetByState(MessageStateTypes state);
 
        void Add(NotificationEvent notification);
        void Delete(Guid notificationId);
    }
}