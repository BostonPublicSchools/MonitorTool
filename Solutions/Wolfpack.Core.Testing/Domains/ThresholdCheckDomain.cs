using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.Testing.Drivers;

namespace Wolfpack.Core.Testing.Domains
{
    public class ThresholdCheckDomainConfig : PluginConfigBase, ISupportNotificationMode, ISupportNotificationThreshold
    {
        public string CheckName { get; set; }

        public string NotificationMode { get; set; }

        public double? NotificationThreshold { get; set; }

        public double[] Values;
        /// <summary>
        /// The interval between firing each value
        /// </summary>
        public int Interval;
        /// <summary>
        /// The number of times to repeat firing of all the values
        /// </summary>
        public int Cycles;

        public static ThresholdCheckDomainConfig FiresValues(params double[] values)
        {
            return new ThresholdCheckDomainConfig
            {
                Values = values,
                Cycles = 1
            };
        }

        public ThresholdCheckDomainConfig CheckNameIs(string name)
        {
            CheckName = CheckName;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval">interval in milliseconds</param>
        /// <returns></returns>
        public ThresholdCheckDomainConfig AtInterval(int interval)
        {
            Interval = interval;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cycles"></param>
        /// <returns></returns>
        public ThresholdCheckDomainConfig Repeat(int cycles)
        {
            Cycles = cycles;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>      
        /// <returns></returns>
        public ThresholdCheckDomainConfig ThresholdIs(double threshold)
        {
            NotificationThreshold = threshold;
            return this;
        }

        public ThresholdCheckDomainConfig NotificationModeIs(string mode)
        {
            NotificationMode = mode;
            return this;
        }
    }

    public class ThresholdCheckDomain : HealthCheckDomain
    {
        protected ThresholdCheckDomainConfig _config;

        public ThresholdCheckDomain(ThresholdCheckDomainConfig config)
        {
            _config = config;
        }

        public ThresholdCheckDomain(ThresholdCheckDomainConfig config, 
            params INotificationRequestFilter[] filters)
            : base(filters)
        {
            _config = config;
        }

        protected override IHealthCheckPlugin HealthCheck
        {
            get { return new AutomationStreamingHealthCheck(_config); }
        }
    }
}