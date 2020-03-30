using System;
using System.Linq;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.Publishers.Sql;

namespace Wolfpack.Core.Repositories.Sql
{
    public class SqlRepository : INotificationRepository
    {
        private readonly ISqlDbContextProvider _provider;
        private readonly SqlPublisherConfiguration _config;

        public SqlRepository(SqlPublisherConfiguration config)
            : this(new SqlDbContextProvider(() => new SqlDbContext(config.ConnectionName)))
        {
            _config = config;
        }

        public SqlRepository(ISqlDbContextProvider provider)
        {
            _provider = provider;
        }

        public void Initialise()
        {
            Logger.Info("SqlPublisher ({0}) initialised!", _config.ConnectionName);
        }

        public IQueryable<NotificationEvent> Filter(params INotificationRepositoryQuery[] filters)
        {
            using (var context = _provider.Provide())
            {
                return filters.Aggregate(context.Notifications.AsQueryable(), (current, filter) => filter.Filter(current));
            }            
        }

        public bool GetById(Guid id, out NotificationEvent notification)
        {
            using (var context = _provider.Provide())
            {
                notification = context.Notifications.FirstOrDefault(n => n.Id.Equals(id));
            }
            return notification != null;
        }

        public IQueryable<NotificationEvent> GetByState(MessageStateTypes state)
        {
            using (var context = _provider.Provide())
            {
                return context.Notifications.Where(n => n.State.Equals(state));
            }
        }

        public void Add(NotificationEvent notification)
        {
            using (var context = _provider.Provide())
            {
                context.Notifications.Add(notification);
                context.SaveChanges();
            }
        }

        public void Delete(Guid notificationId)
        {
            using (var context = _provider.Provide())
            {
                var notification = context.Notifications.FirstOrDefault(n => n.Id.Equals(notificationId));

                if (notification == null)
                    return;

                context.Notifications.Remove(notification);
                context.SaveChanges();
            }
        }
    }
}