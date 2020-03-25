using System;
using System.Data.Entity;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Repositories.Sql
{
    public interface ISqlDbContext : IDisposable
    {
        IDbSet<NotificationEvent> Notifications { get; set; }
        void SaveChanges();
    }
}