using System;
using System.Collections.Generic;
using System.IO;
using Castle.Core.Internal;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Configuration.FileSystem
{
    public class FileSystemScheduleConfigurationRepository : FileSystemConfigurationRepository
    {
        public FileSystemScheduleConfigurationRepository(string baseFolder) : base(baseFolder)
        {
        }

        protected override IEnumerable<FileSystemConfigurationEntry> ProcessEntries(ICollection<FileSystemConfigurationEntry> entries)
        {
            entries.ForEach(e =>
                                {
                                    Logger.Debug("Processing schedule configuration entry: {0}...", e.FileInfo.Name);

                                    var name = Path.GetFileNameWithoutExtension(e.FileInfo.Name);
                                    var concreteType = Type.GetType(e.Entry.ConfigurationType);
                                    var instance = Serialiser.FromJson(e.Entry.Data, concreteType);

                                    Container.RegisterInstance(instance, name);

                                    e.Entry.RequiredProperties
                                        .AddIfMissing(Tuple.Create(ConfigurationEntry.RequiredPropertyNames.NAME, name));
                                });
            return entries;
        }

        public override void Save(ConfigurationChangeRequest change)
        {
            if (!change.Entry.Tags.ContainsAll(SpecialTags.SCHEDULE))
                return;

            var filepath = Path.Combine(BaseFolder, Path.ChangeExtension(change.Entry.Name, CONFIG_FILE_EXTENSION));

            if (!HandleChange(change, filepath))
                throw new InvalidOperationException(string.Format("Unknown ChangeRequest action '{0}'", change.Action));
        }
    }
}