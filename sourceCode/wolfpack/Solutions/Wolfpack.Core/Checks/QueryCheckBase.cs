using System;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.Notification;

namespace Wolfpack.Core.Checks
{
    public abstract class QueryCheckBase<T> : HealthCheckBase<T>
        where T : QueryCheckConfigBase
    {
        protected readonly QueryCheckConfigBase _baseConfig;
        protected PluginDescriptor  _identity;

        /// <summary>
        /// default ctor
        /// </summary>
        protected QueryCheckBase(T config) : base(config)
        {
            _baseConfig = config;
        }

        protected abstract DataTable RunQuery(string query);
        protected abstract string DescribeNotification();

        public override void Execute()
        {           
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var data = RunQuery(_config.Query);

            stopwatch.Stop();

            ArtifactDescriptor artifactDescriptor = null;

            if (_config.GenerateArtifacts.GetValueOrDefault(false))
                artifactDescriptor = ArtifactManager.Save(Identity, ArtifactManager.KnownContentTypes.TabSeparated, data);

            Publish(data.Rows.Count, stopwatch.Elapsed, artifactDescriptor);
        }

        /// <summary>
        /// Allows a subclass to validate its config properties
        /// </summary>
        protected virtual void ValidateConfig()
        {
            // do nothing
        }

        /// <summary>
        /// Implements logic to decide whether this query produces a successful or not result
        /// </summary>
        /// <param name="rowcount"></param>
        /// <returns></returns>
        protected virtual bool DecideResult(int rowcount)
        {
            bool result;

            if (_baseConfig.InterpretZeroRowsAsAFailure)
            {
                result = (rowcount > 0);
            }
            else
            {
                result = (rowcount == 0);
            }

            return result;
        }

        /// <summary>
        /// Override this if you want to alter the publishing behaviour
        /// </summary>
        /// <param name="rowcount"></param>
        /// <param name="duration"></param>
        /// <param name="artifactDescriptor"></param>        
        protected virtual void Publish(int rowcount, TimeSpan duration, ArtifactDescriptor artifactDescriptor)
        {
            var data = HealthCheckData.For(Identity, DescribeNotification())
                .ResultIs(DecideResult(rowcount))
                .ResultCountIs(rowcount)
                .SetDuration(duration)
                .AddProperty("Rowcount", rowcount.ToString(CultureInfo.InvariantCulture))
                .AddProperty("Criteria", _baseConfig.Query);

            Publish(NotificationRequestBuilder.For(_config.NotificationMode, data)
                .AssociateArtifact(artifactDescriptor)
                .Build());
        }        
    }
}