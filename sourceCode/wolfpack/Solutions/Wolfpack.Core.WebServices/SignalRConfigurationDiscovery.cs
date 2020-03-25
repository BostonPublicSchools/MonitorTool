using Wolfpack.Core.Configuration;
using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.WebServices.Interfaces.Entities;

namespace Wolfpack.Core.WebServices
{
    public class SignalRConfigurationDiscovery : PluginDiscoveryBase<SignalRActivityConfig, SignalRActivity>
    {
        protected override SignalRActivityConfig GetConfiguration()
        {
            return new SignalRActivityConfig
                       {
                           Enabled = true,
                           BaseUrl = "http://*:803/"
                       };
        }

        protected override void Configure(ConfigurationEntry entry)
        {
            entry.Name = "SignalRActivity";
            entry.Description = "This activity hosts SignalR - Wolfpack notifications are rebroadcast to all connected clients.";
            entry.Tags.AddIfMissing(SpecialTags.ACTIVITY, "SignalR", "Realtime");
        }
    }
}