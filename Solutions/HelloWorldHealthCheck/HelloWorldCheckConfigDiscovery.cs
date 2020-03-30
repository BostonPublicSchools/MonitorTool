using Wolfpack.Core;
using Wolfpack.Core.Configuration;
using Wolfpack.Core.Interfaces.Entities;

namespace HelloWorldHealthCheck
{
    public class HelloWorldCheckConfigDiscovery : HealthCheckDiscoveryBase<HelloWorldCheckConfig, HelloWorldCheck>
    {
        protected override HelloWorldCheckConfig GetConfiguration()
        {
            return new HelloWorldCheckConfig
            {
                Enabled = true,
                FriendlyId = "CHANGEME!",
                MyCustomSetting = "HelloWorld was ere!"
            };
        }

        protected override void Configure(ConfigurationEntry entry)
        {
            entry.Name = "HelloWorld";
            entry.Description = "My first custom Wolfpack plugin!";
            entry.Tags.AddIfMissing("HelloWorld");
        }
    }
}