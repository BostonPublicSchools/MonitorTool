using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using Wolfpack.Core.Configuration;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.Notification;
using Wolfpack.Core.Notification.Filters.Request;

namespace Wolfpack.Core.Checks
{
    public class WindowsServiceStartupCheckConfig : PluginConfigBase, ISupportNotificationMode
    {
        /// <summary>
        /// List of windows services to check the startup type of. This can be
        /// the Display name or Short name of the service.
        /// </summary>
        public List<string> Services { get; set; }
        /// <summary>
        /// The server to monitor the windows services on
        /// </summary>
        public string Server { get; set; }
        /// <summary>
        /// The Startup Type you expect these services to be in; values are
        /// <list type="bullet">
        /// <item><description>Auto</description></item>
        /// <item><description>Manual</description></item>
        /// <item><description>Disabled</description></item>
        /// </list>
        /// </summary>
        public string ExpectedStartupType { get; set; }

        /// <summary>
        /// Helper method to quickly validate the <see cref="ExpectedStartupType"/>
        /// configuration value is valid
        /// </summary>
        /// <returns></returns>
        public bool StartupTypeIsValid()
        {
            return ParameterValueIsInList(ExpectedStartupType, "auto", "manual", "disabled");
        }

        public string NotificationMode { get; set; }
    }

    public class WindowsServiceStartupConfigurationAdvertiser : HealthCheckDiscoveryBase<WindowsServiceStartupCheckConfig, WindowsServiceStartupCheck>
    {
        protected override WindowsServiceStartupCheckConfig GetConfiguration()
        {
            return new WindowsServiceStartupCheckConfig
                       {
                           Enabled = true,
                           ExpectedStartupType = "Auto|Manual|Disabled",
                           FriendlyId = "CHANGEME!",
                           NotificationMode = StateChangeNotificationFilter.FilterName,
                           Server = "localhost",
                           Services = new List<string>
                                          {
                                              "DHCP Client",
                                              "Computer Browser"
                                          }
                       };
        }

        protected override void Configure(ConfigurationEntry entry)
        {
            entry.Name = "WindowsServiceStartupCheck";
            entry.Description = "This check will monitor the startup mode of a set of Windows Services.";
            entry.Tags.AddIfMissing("Services");
        }
    }

    /// <summary>
    /// This HealthCheck will monitor a list of windows services and checks that
    /// they are have the startup type specified in the configuration.
    /// It will only publish as HealthCheck result if the service does not have the 
    /// correct startup type. A HealthCheck result is published for EACH service that 
    /// fails the check.
    /// </summary>
    /// <remarks>This HealthCheck uses WMI to return information about the Windows Services,
    /// the .Net ServiceController wrapper does not return the Startup Type so we have to
    /// use WMI to do this</remarks>
    public class WindowsServiceStartupCheck : HealthCheckBase<WindowsServiceStartupCheckConfig>
    {
        protected readonly string Server;
        protected readonly string WmiNamespace;

        public WindowsServiceStartupCheck(WindowsServiceStartupCheckConfig config) 
            : base(config)
        {
            Server = string.IsNullOrEmpty(config.Server) ? "." : config.Server;
            WmiNamespace = string.Format(@"\\{0}\root\cimv2", Server);
        }

        public override void Initialise()
        {
            Logger.Debug("Initialising WindowsServiceStartup check for...");

            if (!_config.StartupTypeIsValid())
                throw new FormatException(
                    string.Format("Value '{0}' for configuration property 'ExpectedStartupType' is not valid",
                                  _config.ExpectedStartupType));

            _config.Services.ForEach(service => Logger.Debug("\t{0}", service));
            Logger.Debug("\tComplete, monitoring services for expected startup type '{0}' on Server '{1}'",
                _config.ExpectedStartupType, Server);
        }

        public override void Execute()
        {
            Logger.Debug("WindowsServiceStartupCheck is checking service startup types...");

            // use the service/current identity to query local or remote
            var wmiScope = new ManagementScope(WmiNamespace, new ConnectionOptions
                      {
                          Impersonation = ImpersonationLevel.Impersonate
                      });

            // set up the query and execute it
            var wmiQuery = new ObjectQuery("Select * from Win32_Service");
            var wmiSearcher = new ManagementObjectSearcher(wmiScope, wmiQuery);
            var wmiResults = wmiSearcher.Get();
            var results = wmiResults.Cast<ManagementObject>();
            var services = (from service in results
                           select new
                                      {
                                          Name = service["Name"].ToString(),
                                          DisplayName = service["DisplayName"].ToString(),
                                          StartMode = service["StartMode"].ToString()
                                      }).ToList();

            // find the services we are interested in that do not have the
            // startup type (startmode) we expect
            var faultedServices = (from service in services
                                   from name in _config.Services
                                   where MatchName(name, service.Name, service.DisplayName)
                                          && (string.Compare(_config.ExpectedStartupType, service.StartMode, 
                                          StringComparison.OrdinalIgnoreCase) != 0)
                                   select service).ToList();

            faultedServices.ForEach(
                fs =>
                    {
                        var result = HealthCheckData.For(Identity, string.Format("{0} should be {1} but is {2}",
                            fs.DisplayName, _config.ExpectedStartupType, fs.StartMode))
                            .AddTag(fs.DisplayName)
                            .Failed();

                        var request = NotificationRequestBuilder.For(_config.NotificationMode, result,
                            nr =>
                            {
                                nr.DataKeyGenerator = data => string.Format("{0}>>{1}", data.CheckId, data.Tags);
                            }).Build();

                        Publish(request);
                    });

            // invalid/unknown service names...
            var invalidServices = _config.Services.Where(configService =>
                !services.Any(svc => MatchName(configService, svc.Name, svc.DisplayName))).ToList();

            if (invalidServices.Any())
                throw new InvalidOperationException(string.Format("These services do not exist: {0}",
                    string.Join(",", invalidServices.ToArray())));
        }

        protected override PluginDescriptor BuildIdentity()
        {
            return new PluginDescriptor
            {
                Description = string.Format("Ensures all specified windows services have the expected startup type '{0}'", _config.ExpectedStartupType),
                TypeId = new Guid("F3EA664E-B753-471f-87F4-C15D0395B11A"),
                Name = _config.FriendlyId
            };
        }

        private static bool MatchName(string requiredService, string name, string displayName)
        {
            return (string.Compare(requiredService, name, StringComparison.OrdinalIgnoreCase) == 0) ||
                   (string.Compare(requiredService, displayName, StringComparison.OrdinalIgnoreCase) == 0);
        }
    }
}