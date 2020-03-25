using System;
using System.Linq;
using NUnit.Framework;
using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.Repositories.Plugins;
using Wolfpack.Core.Repositories.Queries;
using Wolfpack.Core.Repositories.Sql;
using Wolfpack.Core.Testing;
using Wolfpack.Core.Testing.Bdd;

namespace Wolfpack.Tests.Repositories
{
    public class SqlRepositoryDomain : BddTestDomain
    {
        private readonly SqlDbContext _dbContext;
        private readonly SqlRepository _sut;

        private IQueryable<NotificationEvent> _queryResults;

        public override void Dispose()
        {           
        }

        public SqlRepositoryDomain()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);
            _dbContext = new SqlDbContext("Wolfpackv3");
            _sut = new SqlRepository(new SqlDbContextProvider(() => _dbContext));
        }

        public void TheWolfpackDatabaseIsClearedOfNotifications()
        {
            _dbContext.Database.ExecuteSqlCommand("DELETE FROM CriticalFailureDetails");
            _dbContext.Database.ExecuteSqlCommand("DELETE FROM NotificationEvents");
        }

        public void NotificationWithId_IsAddedToTheRepository(Guid id)
        {
            var notification = NotificationEventBuilder.From(new NotificationEventHealthCheck())
                .Build();
            notification.Id = id;

            _sut.Add(notification);
        }

        public void TheNotificationWithId_ShouldBeInTheNotificationTable(Guid id)
        {
            Assert.That(_sut.Filter(new NotificationByIdQuery(id)).Count(), Is.EqualTo(1));
        }

        public void TheQueryForId_AndState_IsExecuted(Guid id, MessageStateTypes state)
        {
            _queryResults = _sut.Filter(new NotificationByIdQuery(id), new NotificationByStateQuery(state));
        }

        public void TheQueryResultsShouldHave_Items(int expected)
        {
            Assert.That(_queryResults.Count(), Is.EqualTo(expected));
        }
    }
}