using System.Data.Entity;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Repositories.Sql
{
    public class SqlDbContext : DbContext, ISqlDbContext
    {
        public IDbSet<NotificationEvent> Notifications { get; set; }

        public SqlDbContext()
        {
        }

        public SqlDbContext(string connectionName) 
            : base(connectionName)
        {
        }

        public new void SaveChanges()
        {
            base.SaveChanges();
        }
    }
}