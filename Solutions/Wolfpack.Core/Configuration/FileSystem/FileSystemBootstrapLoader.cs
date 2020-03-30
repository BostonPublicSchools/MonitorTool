using System;
using System.Collections.Generic;
using System.IO;
using Castle.Core.Internal;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Configuration.FileSystem
{
    public class FileSystemBootstrapLoader : FileSystemConfigurationRepository
    {
        public FileSystemBootstrapLoader(string baseFolder)
            : base(baseFolder)
        {
        }

        protected override IEnumerable<FileSystemConfigurationEntry> ProcessEntries(ICollection<FileSystemConfigurationEntry> entries)
        {
            entries.ForEach(
                e =>
                    {
                        Logger.Debug("Processing bootstrap configuration entry: {0}...", e.FileInfo.Name);

                        var configType = Type.GetType(e.Entry.ConfigurationType);
                        var config = Serialiser.FromJson(e.Entry.Data, configType);

                        if (IsDisabled(config))
                            return;

                        var name = Path.GetFileNameWithoutExtension(e.FileInfo.Name);
                        Container.RegisterInstance(configType, config, name);
                        FindAndExecuteBootstrappers(configType, config);
                        e.Entry.RequiredProperties.AddIfMissing(Tuple.Create(ConfigurationEntry.RequiredPropertyNames.NAME, name));

                        
                        if (string.IsNullOrWhiteSpace(e.Entry.PluginType))
                            return;

                        var interfaceType = string.IsNullOrWhiteSpace(e.Entry.InterfaceType)
                            ? null
                            : Type.GetType(e.Entry.InterfaceType);
                        var pluginType = Type.GetType(e.Entry.PluginType);
                        if (pluginType == null)
                        {
                            Logger.Warning("Unable to create pluginType '{0}'", e.Entry.PluginType);
                            return;
                        }

                        var plugin = Activator.CreateInstance(pluginType, config);

                        if (interfaceType != null)
                            Container.RegisterInstance(interfaceType, plugin);
                        else
                            Container.RegisterInstance(plugin);
                    });

            return entries;
        }

        public override void Save(ConfigurationChangeRequest change)
        {
            if (!change.Entry.Tags.ContainsAll(SpecialTags.BOOTSTRAP))
                return;

            var filepath = Path.Combine(BaseFolder, Path.ChangeExtension(change.Entry.Name, CONFIG_FILE_EXTENSION));

            if (!HandleChange(change, filepath))
                throw new InvalidOperationException(string.Format("Unknown ChangeRequest action '{0}'", change.Action));
        }
    }
}