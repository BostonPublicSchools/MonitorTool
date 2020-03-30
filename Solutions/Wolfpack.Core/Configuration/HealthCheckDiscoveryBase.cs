using System;
using System.Collections.Generic;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Configuration
{
    public abstract class HealthCheckDiscoveryBase<TConfig, TPlugin> : PluginDiscoveryBase<TConfig, TPlugin>
    {
        public override List<string> GetTags()
        {
            return new List<string>{ SpecialTags.HEALTH_CHECK };
        }

        public override Properties GetRequiredProperties()
        {
            var props = base.GetRequiredProperties();
            props.AddIfMissing(Tuple.Create(ConfigurationEntry.RequiredPropertyNames.SCHEDULER, "** CHANGEME ** (connects this check with an existing schedule)"));
            return props;
        }
    }
}