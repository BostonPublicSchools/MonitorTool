using System;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Repositories.Sql;

namespace Wolfpack.Core.Publishers.Sql
{
    public class SqlPublisherBootstrapper : ISupportBootStrapping<SqlPublisherConfiguration>
    {
        public void Execute(SqlPublisherConfiguration config)
        {
            if (!config.Enabled)
                return;

            // some magic sauce to make the AttachDbFilename connectionstring property 
            // accept "relative path" filenames using the |DataDirectory| token
            AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);

            if (config.UseAsRepository)
            {
                Container.RegisterInstance<ISqlDbContextProvider>(
                    new SqlDbContextProvider(() => new SqlDbContext(config.ConnectionName)));
                Container.RegisterAsTransient<INotificationRepository>(typeof(SqlRepository));
            }
        }
    }
}