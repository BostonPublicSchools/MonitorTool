using System.Collections.Generic;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Configuration
{
    public abstract class PluginDiscoveryBase<TConfig> : ISupportConfigurationDiscovery
    {
        protected abstract TConfig GetConfiguration();
        protected abstract void Configure(ConfigurationEntry entry);

        public virtual Properties GetRequiredProperties()
        {
            return new Properties
                       {
                           {ConfigurationEntry.RequiredPropertyNames.NAME, typeof (TConfig).Name}
                       };
        }

        public virtual List<string> GetTags()
        {
            return new List<string>();
        }

        public virtual string GetLink()
        {
            return string.Empty;
        }

        public virtual ConfigurationEntry GetConfigurationMetadata()
        {
            var entry = BuildBasicEntry();
            Configure(entry);
            return entry;
        }

        protected virtual ConfigurationEntry BuildBasicEntry()
        {
            var name = string.Format("{0}Config", typeof(TConfig).Name);

            return new ConfigurationEntry
            {
                Name = name,
                Description = string.Format("Configuration settings for '{0}'", name),
                Tags = GetTags(),
                Link = GetLink(),
                RequiredProperties = GetRequiredProperties(),
                Data = Serialiser.ToJson(GetConfiguration()),
                ConfigurationType = BuildTypeName<TConfig>(),
            };
        }

        protected string BuildTypeName<T>()
        {
            var type = typeof (T);
            return string.Format("{0}, {1}", type.FullName, type.Assembly.GetName().Name);
        }
    }

    public abstract class PluginDiscoveryBase<TConfig, TPlugin> : PluginDiscoveryBase<TConfig>
    {
        protected override ConfigurationEntry BuildBasicEntry()
        {
            var entry = base.BuildBasicEntry();
            entry.PluginType = BuildTypeName<TPlugin>();
            return entry;
        }
    }
    public abstract class PluginDiscoveryBase<TConfig, TPlugin, TInterface> : PluginDiscoveryBase<TConfig, TPlugin>
    {
        protected override ConfigurationEntry BuildBasicEntry()
        {
            var entry = base.BuildBasicEntry();
            entry.InterfaceType = BuildTypeName<TInterface>();
            return entry;
        }
    }
}