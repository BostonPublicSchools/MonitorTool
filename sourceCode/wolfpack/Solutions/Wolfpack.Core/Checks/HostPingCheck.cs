using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Wolfpack.Core.Configuration;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.Notification;
using Wolfpack.Core.Notification.Filters.Request;

namespace Wolfpack.Core.Checks
{
    public class HostPingCheckConfig : PluginConfigBase, ISupportNotificationMode, ISupportNotificationThreshold
    {
        public List<string> Hosts { get; set; } 
        public string NotificationMode { get; set; }
        public double? NotificationThreshold { get; set; }
    }

    public class HostPingCheckConfigurationAdvertiser : HealthCheckDiscoveryBase<HostPingCheckConfig, HostPingCheck>
    {
        protected override HostPingCheckConfig GetConfiguration()
        {
            return new HostPingCheckConfig
                       {
                           Enabled = true,
                           FriendlyId = "HostPing",
                           Hosts = new List<string> {"localhost", "AddMoreServersHere"},
                           NotificationMode = StateChangeNotificationFilter.FilterName,
                           NotificationThreshold = 100
                       };
        }

        protected override void Configure(ConfigurationEntry entry)
        {
            entry.Name = "HostPing";
            entry.Description =
                "Pings a host with an ICMP message and raises an alert if the response is not received or is too slow.";
            entry.Tags.AddIfMissing("ICMP", "Ping");
        }
    }

    /// <summary>
    /// Pings a set of hosts with an ICMP message
    /// </summary>
    public class HostPingCheck : ThresholdCheckBase<HostPingCheckConfig>
    {
        /// <summary>
        /// default ctor
        /// </summary>
        public HostPingCheck(HostPingCheckConfig config)
            : base(config)
        {
        }
       
        public override void Initialise()
        {
            Logger.Debug("Initialising HostPingCheck check for...");
            _config.Hosts.ForEach(host => Logger.Debug("\t{0}", host));
        }

        public override void Execute()
        {
            Logger.Debug("HostPingCheck is pinging...");

            _config.Hosts.ForEach(host =>
                                {
                                    using (var pinger = new Ping())
                                    {
                                        var reply = _config.NotificationThreshold.HasValue
                                                              ? pinger.Send(host, (int)_config.NotificationThreshold.Value) 
                                                              : pinger.Send(host);

                                        var data = HealthCheckData.For(Identity, "Successfully pinged host '{0}'",
                                                                         host)
                                            .Succeeded()
                                            .ResultCountIs(reply.RoundtripTime)
                                            .DisplayUnitIs("ms")
                                            .AddProperty("host", host)
                                            .AddProperty("status", reply.Status.ToString());

                                        if (reply.Status != IPStatus.Success)
                                        {
                                            data.Info = string.Format("Failure ({0}) pinging host {1}", reply.Status, host);
                                            data.Failed();
                                        }

                                        Publish(NotificationRequestBuilder.For(_config.NotificationMode, data, 
                                            BuildKeyWithHostName).Build());
                                    }
                                });
        }

        private static void BuildKeyWithHostName(NotificationRequest request)
        {
            request.DataKeyGenerator = (message => string.Format("{0}_{1}", message.CheckId, 
                message.Properties["host"]));
        }

        protected override PluginDescriptor BuildIdentity()
        {
            return new PluginDescriptor
                       {
                           Name = _config.FriendlyId,
                           Description = "Pings a host with an ICMP message and raises an alert if no response is returned",
                           TypeId = new Guid("F29826F2-8930-4E76-8710-47F0C08A4461")
                       };
        }
    }
}
