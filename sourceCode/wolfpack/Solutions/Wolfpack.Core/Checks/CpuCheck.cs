using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Castle.Core.Internal;
using Wolfpack.Core.Configuration;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;
using System.Linq;
using Wolfpack.Core.Notification;
using Wolfpack.Core.Notification.Filters.Request;

namespace Wolfpack.Core.Checks
{
    public class CpuCheckConfig : PluginConfigBase, ISupportNotificationMode, ISupportNotificationThreshold
    {
        public string MachineName { get; set; }
        public string NotificationMode { get; set; }
        public double? NotificationThreshold { get; set; }
    }

    public class CpuCheckConfigurationAdvertiser : HealthCheckDiscoveryBase<CpuCheckConfig, CpuCheck>
    {
        protected override CpuCheckConfig GetConfiguration()
        {
            return new CpuCheckConfig
                       {
                           Enabled = true,
                           FriendlyId = "CHANGEME!",
                           NotificationMode = StateChangeNotificationFilter.FilterName,
                           NotificationThreshold = 80
                       };
        }

        protected override void Configure(ConfigurationEntry entry)
        {
            entry.Name = "CpuCheck";
            entry.Description = "This check monitors the CPU % utilisation level. Typically this check is configured to start producing notifications once a threshold % is breached.";
            entry.Tags.AddIfMissing("CPU", "Threshold");
        }
    }

    public class CpuCheck : ThresholdCheckBase<CpuCheckConfig>
    {
        private class ProcessCounterSample
        {
            public Process Process { get; set; }
            public PerformanceCounter Counter { get; set; }
            public CounterSample Sample1 { get; set; }
            public double Value { get; set; }
        }

        protected PerformanceCounter _counter;

        /// <summary>
        /// default ctor
        /// </summary>
        public CpuCheck(CpuCheckConfig config)
            : base(config)
        {
        }

        public override void  Initialise()
        {
            _counter = string.IsNullOrEmpty(_config.MachineName) 
                ? new PerformanceCounter("Processor", "% Processor Time", "_Total") 
                : new PerformanceCounter("Processor", "% Processor Time", "_Total", _config.MachineName);
        }

        public override void Execute()
        {
            var sample = _counter.NextSample();
            Thread.Sleep(1000);
            var sample2 = _counter.NextSample();
            var value = Math.Round(CounterSampleCalculator.ComputeCounterValue(sample, sample2));

            Publish(NotificationRequestBuilder.For(_config.NotificationMode, HealthCheckData.For(Identity, "Cpu utilisation is {0}%", value)
                .Succeeded()
                .ResultCountIs(value)
                .DisplayUnitIs("%"))
                .Build());

            Logger.Debug("CpuCheck reports {0}%", value);
        }

        /// <summary>
        /// When a breach of the threshold is detected we want to send a list 
        /// the top 5 cpu hogging processes in the alert
        /// </summary>
        /// <param name="message"></param>
        protected override void AdjustMessageForBreach(INotificationEventCore message)
        {
            base.AdjustMessageForBreach(message);

            var counters = new List<ProcessCounterSample>();
            var ps = Process.GetProcesses();

            ps.ForEach(process =>
                           {
                               try
                               {
                                   var counter = (string.IsNullOrEmpty(_config.MachineName)
                                                      ? new PerformanceCounter("Process", "% Processor Time", process.ProcessName)
                                                      : new PerformanceCounter("Process", "% Processor Time", process.ProcessName, _config.MachineName));
                                   counter.ReadOnly = true;

                                   counters.Add(new ProcessCounterSample
                                                    {
                                                        Counter = counter,
                                                        Process = process,
                                                        Sample1 = counter.NextSample()
                                                    });
                               }
                               catch
                               {
                                   Logger.Debug("*** Process '{0}' has no Performance Counter ***", process.ProcessName);
                               }
                           });

            Logger.Info("Getting CPU% for {0} processes...", counters.Count);
            Thread.Sleep(1000);

            counters.ForEach(pcs =>
                                 {
                                     var sample2 = pcs.Counter.NextSample();
                                     pcs.Value = Math.Round(CounterSampleCalculator.ComputeCounterValue(pcs.Sample1, sample2));
                                 });

            counters.Where(pcs => pcs.Value > 0)
                .OrderByDescending(pcs => pcs.Value)
                .Take(5)
                .ForEach(pcs =>
                             {
                                 var name = string.Format("{0}[{1}]", pcs.Process.ProcessName, pcs.Process.Id);
                                 // TODO: add property to core notification properties?
                                 //message.AddProperty(name, Convert.ToString(pcs.Value));
                             });
        }

        protected override PluginDescriptor BuildIdentity()
        {
            return new PluginDescriptor
            {
                Description = "Reports the CPU load as a %",
                TypeId = new Guid("E6C9C3DB-E234-407B-9949-2BE94C185D72"),
                Name = _config.FriendlyId
            };
        }
    }
}