using System;
using System.Collections.Generic;
using System.IO;
using Castle.Core.Internal;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Configuration.FileSystem
{
    public class FileSystemActivityLoader : FileSystemConfigurationRepository
    {
        public FileSystemActivityLoader(string baseFolder)
            : base(baseFolder)
        {
        }

        protected override IEnumerable<FileSystemConfigurationEntry> ProcessEntries(ICollection<FileSystemConfigurationEntry> entries)
        {
            entries.ForEach(
                e =>
                    {
                        Logger.Debug("Processing activity configuration entry: {0}...", e.FileInfo.Name);

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

                        var pluginType = Type.GetType(e.Entry.PluginType);
                        var pluginName = string.Format("{0}.{1}", pluginType.AssemblyQualifiedName, name);
                        Container.RegisterAsSingleton<IActivityPlugin>(pluginType, pluginName,
                            new Tuple<Type, string>(configType, name));
                    });

            return entries;
        }

        public override void Save(ConfigurationChangeRequest change)
        {
            if (!change.Entry.Tags.ContainsAll(SpecialTags.ACTIVITY))
                return;

            var filepath = Path.Combine(BaseFolder, Path.ChangeExtension(change.Entry.Name, CONFIG_FILE_EXTENSION));

            if (!HandleChange(change, filepath))
                throw new InvalidOperationException(string.Format("Unknown ChangeRequest action '{0}'", change.Action));
        }
    }
}