using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.Interfaces.Magnum;
using Wolfpack.Core.Publishers;

namespace Wolfpack.Core.Database.SQLite
{
    public abstract class SQLitePublisherBase : PublisherBase
    {
        protected readonly SQLiteConfiguration myConfig;

        protected SQLitePublisherBase(SQLiteConfiguration config)
        {
            myConfig = config;

            Enabled = config.Enabled;
            FriendlyId = config.FriendlyId;
        }

        public override void Initialise()
        {
            var dbFile = ExtractFilename(SmartConnectionString.For(myConfig.ConnectionString));

            try
            {
                if (!File.Exists(dbFile))
                {
                    Logger.Debug("\tCreating Wolfpack SQLite datafile at {0}...", dbFile);
                    SQLiteConnection.CreateFile(dbFile);
                }

                var schema = GetSchema("AgentData");

                if (schema.Columns.Count == 0)
                {
                    Logger.Debug("\tCreating AgentData table...");
                    using (var cmd = SQLiteAdhocCommand.UsingSmartConnection(myConfig.ConnectionString)
                        .WithSql(SQLiteStatement.Create("CREATE TABLE AgentData (")
                                     .Append("[TypeId] UNIQUEIDENTIFIER NOT NULL,")
                                     .Append("[EventType] TEXT NOT NULL,")
                                     .Append("[SiteId] TEXT NOT NULL,")
                                     .Append("[AgentId] TEXT NOT NULL,")
                                     .Append("[CheckId] TEXT NULL,")
                                     .Append("[Result] BOOL NULL,")
                                     .Append("[GeneratedOnUtc] DATETIME NOT NULL,")
                                     .Append("[ReceivedOnUtc] DATETIME NOT NULL,")
                                     .Append("[Data] TEXT NOT NULL,")
                                     .Append("[Tags] TEXT NULL,")
                                     .Append("[Version] UNIQUEIDENTIFIER NOT NULL,")
                                     .Append("[ResultCount] REAL NULL,")
                                     .Append("[MinuteBucket] INTEGER NULL,")
                                     .Append("[HourBucket] INTEGER NULL,")
                                     .Append("[DayBucket] INTEGER NULL)")))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                AddColumnIfMissing("MinuteBucket", "INTEGER", true);
                AddColumnIfMissing("HourBucket", "INTEGER", true);
                AddColumnIfMissing("DayBucket", "INTEGER", true);
                // Geo - point
                AddColumnIfMissing("Latitude", "TEXT", true);
                AddColumnIfMissing("Longitude", "TEXT", true);

                Logger.Debug("\tSuccess, AgentData table established");
            }
            catch (Exception)
            {
                Logger.Debug("\tError during SQLite datafile/database creation...");
                throw;
            }
        }

        protected virtual string ExtractFilename(string connectionString)
        {
            var parser = new DbConnectionStringBuilder { ConnectionString = connectionString };
            return parser["Data Source"].ToString();
        }

        protected virtual TableSchema GetSchema(string table)
        {
            var columns = new List<TableSchema.ColumnDefinition>();

            using (var cmd = SQLiteAdhocCommand.UsingSmartConnection(myConfig.ConnectionString)
                .WithSql(SQLiteStatement.Create("PRAGMA table_info('{0}')", table)))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var def = new TableSchema.ColumnDefinition
                                      {
                                          Name = reader["name"].ToString(),
                                          Type = reader["type"].ToString(),
                                          IsNullable = Convert.ToBoolean(reader["notnull"])
                                      };
                        columns.Add(def);
                    }
                }
            }

            return new TableSchema
                       {
                           Columns = columns.AsReadOnly()
                       };
        }

        protected virtual bool AddColumnIfMissing(string column, string datatype, bool nullable)
        {
            if (GetSchema("AgentData").HasColumn(column))
                return false;

            using (var cmd = SQLiteAdhocCommand.UsingSmartConnection(myConfig.ConnectionString)
                .WithSql(SQLiteStatement.Create("ALTER TABLE AgentData ADD {0} {1} {2} NULL",
                    column,
                    datatype,
                    nullable ? string.Empty : "NOT")))
            {
                cmd.ExecuteNonQuery();
                Logger.Debug("\tAdded column {0} [{1}]", column, datatype);
            }
            return true;
        }
    }

    public class SQLitePublisher : SQLitePublisherBase, INotificationEventPublisher
    {
        public SQLitePublisher(SQLiteConfiguration config)
            : base(config)
        {
        }

        public void Consume(NotificationEvent message)
        {
            throw new NotImplementedException();
            //var statement = SQLiteStatement.Create("INSERT INTO AgentData (")
            //    .Append(
            //        "TypeId,EventType,SiteId,AgentId,CheckId,Result,ResultCount,GeneratedOnUtc,ReceivedOnUtc,Data,Tags,Version,MinuteBucket,HourBucket,DayBucket")
            //    .AppendIf(() => (message.Check.Geo != null), ",Longitude,Latitude")
            //    .Append(") VALUES (")
            //    .InsertParameter("@pTypeId", message.TypeId).Append(",")
            //    .InsertParameter("@pEventType", message.EventType).Append(",")
            //    .InsertParameter("@pSiteId", message.SiteId).Append(",")
            //    .InsertParameter("@pAgentId", message.AgentId).Append(",")
            //    .InsertParameter("@pCheckId", message.CheckId).Append(",")
            //    .InsertParameter("@pResult", message.Result).Append(",")
            //    .InsertParameter("@pResultCount", message.ResultCount).Append(",")
            //    .InsertParameter("@pGeneratedOnUtc", message.GeneratedOnUtc).Append(",")
            //    .InsertParameter("@pReceivedOnUtc", message.ReceivedOnUtc).Append(",")
            //    .InsertParameter("@pData", message.Data).Append(",")
            //    .InsertParameter("@pTags", message.Tags).Append(",")
            //    .InsertParameter("@pVersion", message.Id).Append(",")
            //    .InsertParameter("@pMinuteBucket", message.MinuteBucket).Append(",")
            //    .InsertParameter("@pHourBucket", message.HourBucket).Append(",")
            //    .InsertParameter("@pDayBucket", message.DayBucket);

            //if (message.Check.Geo != null)
            //{
            //    statement.Append(",")
            //        .InsertParameter("@pLongitude", message.Check.Geo.Longitude).Append(",")
            //        .InsertParameter("@pLatitude", message.Check.Geo.Latitude);
            //}
            //statement.Append(")");

            //using (var cmd = SQLiteAdhocCommand.UsingSmartConnection(myConfig.ConnectionString)
            //    .WithSql(statement))
            //{
            //    cmd.ExecuteNonQuery();
            //}
        }
    }
}