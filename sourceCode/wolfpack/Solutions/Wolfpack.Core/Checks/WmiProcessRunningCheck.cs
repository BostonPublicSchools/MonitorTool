using System;
using System.Linq;
using System.Management;
using Wolfpack.Core.Configuration;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.Notification;
using Wolfpack.Core.Notification.Filters.Request;

namespace Wolfpack.Core.Checks
{
    public class WmiProcessRunningCheckConfig : PluginConfigBase, ISupportNotificationMode
    {
        public string RemoteUser { get; set; }
        public string RemotePwd { get; set; } 
        public string RemoteMachineId { get; set; } 
        public string ProcessName { get; set; }
        public string NotificationMode { get; set; }
    }

    public class WmiProcessConfigurationAdvertiser : HealthCheckDiscoveryBase<WmiProcessRunningCheckConfig, WmiProcessRunningCheck>
    {
        protected override WmiProcessRunningCheckConfig GetConfiguration()
        {
            return new WmiProcessRunningCheckConfig
                       {
                           Enabled = true,
                           FriendlyId = "CHANGEME!",
                           NotificationMode = StateChangeNotificationFilter.FilterName,
                           ProcessName = "CHANGEME!",
                           RemoteMachineId = "localhost",
                           RemoteUser = string.Empty,
                           RemotePwd = string.Empty
                       };
        }

        protected override void Configure(ConfigurationEntry entry)
        {
            entry.Name = "WmiProcessCheck";
            entry.Description =
                "This check uses WMI to detect if a named process is running. The process can be local or on a remote machine.";
            entry.Tags.AddIfMissing("HealthCheck", "WMI");
        }
    }

    public class WmiProcessRunningCheck : IHealthCheckPlugin
    {
        protected readonly PluginDescriptor PluginIdentity;
        protected readonly string WmiNamespace;
        protected readonly WmiProcessRunningCheckConfig Config;

        public WmiProcessRunningCheck(WmiProcessRunningCheckConfig config)
        {
            Config = config;
            WmiNamespace = string.Format(@"\\{0}\root\cimv2", config.RemoteMachineId);
            PluginIdentity = new PluginDescriptor
            {
                Description = string.Format("Checks for the existance of process '{0}' on {1}", Config.ProcessName, Config.RemoteMachineId),
                TypeId = new Guid("46D4374C-C65D-442e-9B93-AF50BB8C045C"),
                Name = Config.FriendlyId
            };
        }

        public Status Status { get; set; }

        public PluginDescriptor Identity
        {
            get { return PluginIdentity; }
        }

        public void Execute()
        {
            ManagementScope wmiScope;

            Logger.Debug("Querying wmi namespace {0}...", WmiNamespace);
            if (!string.IsNullOrEmpty(Config.RemoteUser) && !string.IsNullOrEmpty(Config.RemotePwd))
            {
                wmiScope = new ManagementScope(WmiNamespace, new ConnectionOptions
                {
                    Username = Config.RemoteUser,
                    Password = Config.RemotePwd
                });
            }
            else
            {
                wmiScope = new ManagementScope(WmiNamespace);
            }

            // set up the query and execute it
            var wmiQuery = new ObjectQuery("Select * from Win32_Process");
            var wmiSearcher = new ManagementObjectSearcher(wmiScope, wmiQuery);
            var wmiResults = wmiSearcher.Get();
            var processes = wmiResults.Cast<ManagementObject>();

            var matches = (from process in processes
                          where (string.Compare(process["Name"].ToString(), Config.ProcessName, 
                          StringComparison.InvariantCultureIgnoreCase) == 0)
                          select process).ToList();

            var data = HealthCheckData.For(Identity, "There are {0} instances of process '{1}' on {2}",
                                          matches.Count(),
                                          Config.ProcessName,
                                          Config.RemoteMachineId)
                .ResultIs(matches.Any())
                .ResultCountIs(matches.Count);

            Messenger.Publish(NotificationRequestBuilder.For(Config.NotificationMode, data).Build());
        }

        public void Initialise()
        {
            // do nothing here
        }
    }
}