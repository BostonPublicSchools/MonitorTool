using System;
using System.Data;
using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.Database.SqlServer;
using System.Diagnostics;
using Wolfpack.Core.Configuration;
using Wolfpack.Core.Notification;
using Wolfpack.Core.Notification.Filters.Request;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Wolfpack.Core.Checks
{
    public class SqlODAQueryCheckConfig : QueryCheckConfigBase
    {
        public string ConnectionString { get; set; }
    }

    public class SqlODAQueryConfigurationAdvertiser : HealthCheckDiscoveryBase<SqlODAQueryCheckConfig, SqlODAQueryCheck>
    {
        protected override SqlODAQueryCheckConfig GetConfiguration()
        {
            return new SqlODAQueryCheckConfig
            {
                Enabled = true,
                FriendlyId = "CHANGEMESQL!",
                NotificationMode = StateChangeNotificationFilter.FilterName,
                ConnectionString = "Data Source=SEIMSSQL2K5;Initial Catalog=Seims;Persist Security Info=True;User ID=****;Password=****"
            };
        }

        protected override void Configure(ConfigurationEntry entry)
        {
            entry.Name = "SQLCheck";
            entry.Description = "This check monitors a specific SQL ODA Query.";
            entry.Tags.AddIfMissing("HealthCheck", "SQL", "ODAQueries");
        }
    }

    public class SqlODAQueryCheck : QueryCheckBase<SqlODAQueryCheckConfig>
    {
        /// <summary>
        /// default ctor
        /// </summary>
        public SqlODAQueryCheck(SqlODAQueryCheckConfig config)
            : base(config)
        {
        }

        protected override void ValidateConfig()
        {
            base.ValidateConfig();

            if (_config.Query.Contains(";"))
                throw new FormatException("Semi-colons are not accepted in Sql from-query statements");
        }

        protected override DataTable RunQuery(string query)
        {
            var data = new DataTable();

            using (var cmd = SqlServerAdhocCommand.UsingSmartConnection(_config.ConnectionString)
                .WithSql(query))
            {
                using (SqlDataReader reader = (SqlDataReader)cmd.ExecuteReader())
                {
                    data.Load(reader);
                }
            }

            return data;
        }

        protected async Task<DataTable> RunQueryAsync(string query, int commandTimeOut)
        {
            var data = new DataTable();
            {
                var da = new SqlDataAdapter(query, _config.ConnectionString);
                da.SelectCommand.CommandTimeout = commandTimeOut;

                await Task.Run(() => { da.Fill(data); });
            }

            return data;
        }

        protected override string DescribeNotification()
        {
            return "Sql ODA Query Check";
        }

        protected override PluginDescriptor BuildIdentity()
        {
            return new PluginDescriptor
            {
                Description = "Sql ODA Query Check",
                TypeId = new Guid("93605A80-B28C-4CE0-8E15-04CF6CFC684F"),
                Name = _baseConfig.FriendlyId, //"TestSQLFriendlyID"
            };
        }

        public async override void Execute()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                var data = await RunQueryAsync(_config.Query, 15);
                stopwatch.Stop();

                ArtifactDescriptor artifactDescriptor = null;

                if (_config.GenerateArtifacts.GetValueOrDefault(false))
                    artifactDescriptor = ArtifactManager.Save(Identity, ArtifactManager.KnownContentTypes.TabSeparated, data);

                string RunDateTime = DateTime.Now.ToString();
                string textMessage = "Total Rows returned: " + data.Rows.Count;

                // Publish(data.Rows.Count, stopwatch.Elapsed, artifactDescriptor);
                Publish(RunDateTime, data.Rows.Count, textMessage, stopwatch.Elapsed, artifactDescriptor);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                var data = new DataTable();

                ArtifactDescriptor artifactDescriptor = null;
                if (_config.GenerateArtifacts.GetValueOrDefault(false))
                    artifactDescriptor = ArtifactManager.Save(Identity, ArtifactManager.KnownContentTypes.TabSeparated, data);

                string RunDateTime = DateTime.Now.ToString();
                string textMessage = "There is an error: " + ex.ToString().Substring(0, 100) + " ...";
                Publish(RunDateTime, 0, textMessage, stopwatch.Elapsed, artifactDescriptor, true);
            }

        }

        protected void Publish(string RunDateTime, int rowcount, string message, TimeSpan duration, ArtifactDescriptor artifactDescriptor, bool isError = false)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_config.ConnectionString);

            var data = HealthCheckData.For(Identity, DescribeNotification())
                .ResultIs(DecideResult(rowcount))
                .ResultCountIs(rowcount)
                .SetDuration(duration)
                //.AddProperty("Rowcount", rowcount.ToString(CultureInfo.InvariantCulture))
                .AddProperty("Criteria", _baseConfig.Query)
                .AddProperty("RunDateTime", RunDateTime)
                .AddProperty("Server", builder.DataSource)
                //.AddProperty("", _config.JobName)
                .AddProperty("Message", message);

            //data.Result = isError;

            Publish(NotificationRequestBuilder.For(_config.NotificationMode, data)
                .AssociateArtifact(artifactDescriptor)
                .Build());
        }
    }
}