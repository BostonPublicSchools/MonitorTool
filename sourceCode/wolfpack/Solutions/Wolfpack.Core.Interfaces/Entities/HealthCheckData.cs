using System;
using System.Collections.Generic;

namespace Wolfpack.Core.Interfaces.Entities
{
    public class HealthCheckData
    {
        public bool? Result { get; set; }
        public double? ResultCount { get; set; }
        public string DisplayUnit { get; set; }
        public bool CriticalFailure { get; set; }
        public CriticalFailureDetails CriticalFailureDetails { get; set; }
        public PluginDescriptor Identity { get; set; }
        public DateTime GeneratedOnUtc { get; set; }
        public GeoData Geo { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime? NextCheckExpected { get; set; }
        public string Info { get; set; }
        public List<string> Tags { get; set; }
        public Properties Properties { get; set; }

        public HealthCheckData()
        {
            GeneratedOnUtc = DateTime.UtcNow;
            CriticalFailure = false;

            Tags = new List<string>();
            Properties = new Properties();
        }

        public static HealthCheckData For(PluginDescriptor identity,
            string info, params object[] args)
        {
            return new HealthCheckData
                       {
                           Identity = identity,
                           Info = string.Format(info, args)
                       };
        }

        public HealthCheckData Succeeded()
        {
            Result = true;
            return this;
        }

        public HealthCheckData Failed()
        {
            Result = false;
            return this;
        }

        public HealthCheckData AddProperty(string name, string value)
        {
            if (Properties == null)
                Properties = new Properties();

            Properties.Add(name, value);
            return this;
        }

        public HealthCheckData AddTag(string tag)
        {
            if (Tags == null)
                Tags = new List<string>();
            Tags.Add(tag);
            return this;
        }

        public HealthCheckData ResultCountIs(double? count)
        {
            ResultCount = count;
            return this;
        }

        public HealthCheckData DisplayUnitIs(string unit)
        {
            DisplayUnit = unit;
            return this;
        }

        public HealthCheckData InfoIs(string format, params object[] args)
        {
            Info = string.Format(format, args);
            return this;
        }

        public HealthCheckData ResultIs(bool result)
        {
            Result = result;
            return this;
        }

        public HealthCheckData SetGeneratedOnUtc(DateTime observationDate)
        {
            GeneratedOnUtc = observationDate;
            return this;
        }

        public HealthCheckData SetDuration(TimeSpan duration)
        {
            Duration = duration;
            return this;
        }
    }
}