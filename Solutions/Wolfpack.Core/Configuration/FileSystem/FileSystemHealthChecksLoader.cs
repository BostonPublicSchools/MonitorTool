using System;
using System.Collections.Generic;
using System.IO;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;
using System.Linq;

namespace Wolfpack.Core.Configuration.FileSystem
{
    public class FileSystemHealthChecksLoader : FileSystemConfigurationRepository
    {
        public FileSystemHealthChecksLoader(string baseFolder)
            : base(baseFolder)
        {
        }

        protected override IEnumerable<FileSystemConfigurationEntry> ProcessEntries(ICollection<FileSystemConfigurationEntry> entries)
        {
            var validEntries = entries.Select(
                e =>
                    {
                        Logger.Debug("Processing healthcheck configuration entry: {0}...", e.FileInfo.Name);

                        var shouldLoad = true;
                        if (e.FileInfo.Directory.FullName.Equals(BaseFolder,
                            StringComparison.OrdinalIgnoreCase))
                        {
                            shouldLoad = false;

                            Logger.Warning(Logger.Event.During("FileSystemHealthChecksLoader.ProcessEntries")
                                .Description("HealthCheck configuration file '{0}' is not following convention - it should be located in a schedule-named sub folder of {1}. ** Configuration has not been loaded **",
                                e.FileInfo.FullName, BaseFolder));
                        }

                        return new
                        {
                            ShouldLoad = shouldLoad,
                            Entry = e
                        };
                    })
                .Where(es => es.ShouldLoad)
                .Select(es => es.Entry).ToList();


            validEntries.ForEach(
                e =>
                    {
                        var name = Path.GetFileNameWithoutExtension(e.FileInfo.Name);
                        var configType = Type.GetType(e.Entry.ConfigurationType);
                        var config = Serialiser.FromJson(e.Entry.Data, configType);
                        var scheduleConfigName = e.FileInfo.Directory.Name;

                        if (IsDisabled(config))
                            return;

                        // probe for convention based component
                        var checkTypeName = configType.Name.Replace("Config", string.Empty);

                        Type checkType;
                        if (GetType<IHealthCheckPlugin>(checkTypeName, out checkType))
                        {
                            // found it...
                            var check = Activator.CreateInstance(checkType, config);

                            // associate with a scheduler component
                            var schedulerConfig = Container.Resolve(scheduleConfigName);
                            var schedulerTypeName = schedulerConfig.GetType().Name.Replace("Config", string.Empty);

                            Type schedulerType;
                            if (!GetType<IHealthCheckSchedulerPlugin>(schedulerTypeName, out schedulerType))
                                throw new InvalidOperationException(string.Format(
                                        "Searching for type name '{0}'; found no matches. Check the name of the folder",
                                        schedulerTypeName));

                            var scheduler = (IHealthCheckSchedulerPlugin)Activator.CreateInstance(schedulerType, check, schedulerConfig);

                            Logger.Debug("\tAdding Scheduler Instance '{0}' running HealthCheck '{1}' named '{2}' to container...",
                                scheduler.GetType().Name, checkTypeName, name);
                            Container.RegisterInstance(scheduler, name);
                        }
                        else
                        {
                            // just register the configuration component, something else 
                            // might be expecting it eg: a bootstrapper component
                            Container.RegisterInstance(config, name);
                        }

                        e.Entry.RequiredProperties.AddIfMissing(
                            Tuple.Create(ConfigurationEntry.RequiredPropertyNames.SCHEDULER, scheduleConfigName),
                            Tuple.Create(ConfigurationEntry.RequiredPropertyNames.NAME, name));
                    });

            return validEntries;
        }

        public override void Save(ConfigurationChangeRequest change)
        {
            if (!change.Entry.Tags.ContainsAll(SpecialTags.HEALTH_CHECK))
                return;

            string scheduler;

            if (!change.Entry.RequiredProperties.TryGetValue(ConfigurationEntry.RequiredPropertyNames.SCHEDULER, out scheduler))
                throw new InvalidOperationException(string.Format("Unable to update configuration, '{0}' property is missing",
                    ConfigurationEntry.RequiredPropertyNames.SCHEDULER));

            PurgeAllExistingItems(change);

            var filepath = Path.Combine(BaseFolder, scheduler, Path.ChangeExtension(change.Entry.Name, CONFIG_FILE_EXTENSION));

            if (!HandleChange(change, filepath))
                throw new InvalidOperationException(string.Format("Unknown ChangeRequest action '{0}'", change.Action));
        }

        private void PurgeAllExistingItems(ConfigurationChangeRequest change)
        {
            var filespec = Path.ChangeExtension(change.Entry.Name, CONFIG_FILE_EXTENSION);

            foreach (var filename in Directory.GetFiles(BaseFolder, filespec, SearchOption.AllDirectories))
                File.Delete(filename);
        }
    }
}