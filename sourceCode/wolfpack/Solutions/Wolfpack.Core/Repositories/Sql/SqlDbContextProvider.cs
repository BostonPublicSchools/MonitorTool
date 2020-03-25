using System;

namespace Wolfpack.Core.Repositories.Sql
{
    /// <summary>
    /// provides fresh instances of a given ISqlDbContext
    /// </summary>
    public class SqlDbContextProvider : ISqlDbContextProvider
    {
        private readonly Func<ISqlDbContext> _providerFunc;

        public SqlDbContextProvider(Func<ISqlDbContext> providerFunc)
        {
            _providerFunc = providerFunc;
        }

        public ISqlDbContext Provide()
        {
            return _providerFunc();
        }
    }
}
