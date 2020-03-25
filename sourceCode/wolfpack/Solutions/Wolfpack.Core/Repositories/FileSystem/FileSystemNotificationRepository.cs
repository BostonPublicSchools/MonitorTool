using System;
using System.IO;
using System.Linq;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Repositories.FileSystem
{
    public class FileSystemNotificationRepository : INotificationRepository
    {
        private static readonly object LockObject = new object();
        private readonly FileSystemNotificationRepositoryConfig _config;

        public FileSystemNotificationRepository(FileSystemNotificationRepositoryConfig config)
        {
            _config = config;
            Directory.CreateDirectory(SmartLocation.GetLocation(_config.BaseFolder));
        }

        public bool GetById(Guid id, out NotificationEvent notification)
        {
            notification = LoadAll().FirstOrDefault(n => n.Id.Equals(id));
            return notification != null;
        }

        public IQueryable<NotificationEvent> GetByState(MessageStateTypes state)
        {
            return LoadAll().Where(n => n.State.Equals(state));
        }

        public void Add(NotificationEvent notification)
        {
            var filename = MakeItemFilename(notification.Id);
            lock (LockObject)
            {
                Serialiser.ToJsonInFile(filename, notification);
            }
            Logger.Debug("Stored Notification ({0}): {1}", notification.EventType, filename);
        }

        private string MakeItemFilename(Guid id)
        {
            return Path.Combine(SmartLocation.GetLocation(_config.BaseFolder),
                Path.ChangeExtension(id.ToString(), "txt"));
        }

        public void Delete(Guid notificationId)
        {
            var filename = MakeItemFilename(notificationId);
            
            lock (LockObject)
            {
                File.Delete(filename);
            }
        }

        public void Initialise()
        {
            // do nothing!
        }

        public IQueryable<NotificationEvent> Filter(params INotificationRepositoryQuery[] filters)
        {
            var result = LoadAll();
            return filters.Aggregate(result, (current, filter) => filter.Filter(current));
        }

        private IQueryable<NotificationEvent> LoadAll()
        {
            // Serializer throws if multiple instances read same file concurrently
            lock (LockObject)
            {
                return
                    Directory.GetFiles(SmartLocation.GetLocation(_config.BaseFolder), "*.*",
                        SearchOption.TopDirectoryOnly)
                        .ToList().Select(Serialiser.FromJsonInFile<NotificationEvent>).AsQueryable();
            }
        }
    }
}