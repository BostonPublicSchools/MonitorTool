using System;
using System.Collections.Generic;
using System.ServiceProcess;
using Wolfpack.Core.Configuration;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.Notification;
using Wolfpack.Core.Notification.Filters.Request;

namespace Wolfpack.Core.Checks
{
    public class WindowsServiceStateCheckConfig : PluginConfigBase, ISupportNotificationMode
    {
        /// <summary>
        /// List of windows services to check the state of. This can be
        /// the Display name or Short name of the service.
        /// </summary>
        public List<string> Services { get; set; }
        /// <summary>
        /// The server to monitor the windows services on
        /// </summary>
        public string Server { get; set; }
        /// <summary>
        /// The state you expect these services to be in; values are
        /// <list type="bullet">
        /// <item><description>Stopped</description></item>
        /// <item><description>Running</description></item>
        /// </list>
        /// </summary>
        public string ExpectedState { get; set; }

        public string NotificationMode { get; set; }

        /// <summary>
        /// Helper method to quickly validate the <see cref="ExpectedState"/>
        /// configuration value is valid
        /// </summary>
        /// <returns></returns>
        public bool StateIsValid()
        {
            return ParameterValueIsInList(ExpectedState, "running", "stopped");
        }
    }

    public class WindowsServiceStateConfigurationAdvertiser : HealthCheckDiscoveryBase<WindowsServiceStateCheckConfig, WindowsServiceStateCheck>
    {
        protected override WindowsServiceStateCheckConfig GetConfiguration()
        {
            return new WindowsServiceStateCheckConfig
                       {
                           Enabled = true,
                           ExpectedState = "Running|Stopped",
                           FriendlyId = "CHANGEME!",
                           NotificationMode = StateChangeNotificationFilter.FilterName,
                           Server = "localhost",
                           Services = new List<string>
                                          {
                                              "DHCP Client",
                                              "DNS Client"
                                          }
                       };
        }

        protected override void Configure(ConfigurationEntry entry)
        {
            entry.Name = "WindowsServiceStateCheck";
            entry.Description = "This check monitors the state of a set of Windows Services. Use it to ensure your mission critical services are up and running (it also supports monitoring that service(s) should be stopped.";
            entry.Tags.AddIfMissing("Services");
        }
    }

    /// <summary>
    /// This HealthCheck will monitor a list of windows services and checks that
    /// they are in the state (Running or Stopped) specified in the configuration.
    /// It will only publish as HealthCheck result if the service is not in the state
    /// expected. A HealthCheck result is published for EACH service that fails the check.
    /// </summary>
    public class WindowsServiceStateCheck : HealthCheckBase<WindowsServiceStateCheckConfig>
    {
        protected readonly string Server;
        protected ServiceControllerStatus ExpectedState;

        public WindowsServiceStateCheck(WindowsServiceStateCheckConfig config)
            : base(config)
        {
            Server = string.IsNullOrEmpty(_config.Server) ? "." : _config.Server;
        }

        public override void Initialise()
        {
            Logger.Debug("Initialising WindowsServiceState check for...");
            if (!_config.StateIsValid())
                throw new FormatException(string.Format("Value '{0}' for configuration property 'ExpectedState' is not valid",
                    _config.ExpectedState));

            // save this for use in the check later
            ExpectedState = (ServiceControllerStatus) Enum.Parse(typeof (ServiceControllerStatus), _config.ExpectedState, true);

            Logger.Debug("\tComplete, monitoring services for expected state '{0}' on Server '{1}'",
                _config.ExpectedState, Server);
        }

        public override void Execute()
        {
            Logger.Debug("WindowsServiceStateCheck is checking service states...");
            var failures = new List<string>();

            _config.Services.ForEach(
                serviceName =>
                    {
                        try
                        {
                            var sc = new ServiceController(serviceName, Server);
                            var result = HealthCheckData.For(Identity,
                                string.Format("{0} is {1}", sc.DisplayName, _config.ExpectedState))
                                .ResultIs(sc.Status == ExpectedState)
                                .AddProperty("ExpectedState", ExpectedState.ToString());

                            var request = NotificationRequestBuilder.For(_config.NotificationMode, result,
                            nr =>
                            {
                                nr.DataKeyGenerator = data => string.Format("{0}>>{1}", data.CheckId, serviceName);
                            }).Build();

                            Publish(request);
                        }
                        catch (InvalidOperationException)
                        {
                            failures.Add(serviceName);
                        }
                    });

            if (failures.Count > 0)
            {
                throw new InvalidOperationException(string.Format("These services do not exist: {0}",
                    string.Join(",", failures.ToArray())));
            }
        }

        protected override PluginDescriptor BuildIdentity()
        {
            return new PluginDescriptor
            {
                Description = string.Format("Ensures all specified windows services are in the expected state {0}", _config.ExpectedState),
                TypeId = new Guid("BEFDC111-ED9A-4aee-A3B9-4251BF9F833A"),
                Name = _config.FriendlyId
            };
        }
    }
}