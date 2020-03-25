using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Configuration.FileSystem
{    
    public abstract class FileSystemConfigurationRepository : IConfigurationRepository
    {
        public const string CONFIG_FILE_EXTENSION = "config";

        protected string BaseFolder;

        protected FileSystemConfigurationRepository(string baseFolder)
        {
            BaseFolder = SmartLocation.GetLocation(baseFolder);
        }
       
        protected abstract IEnumerable<FileSystemConfigurationEntry> ProcessEntries(ICollection<FileSystemConfigurationEntry> entries);

        public abstract void Save(ConfigurationChangeRequest change);

        public virtual IEnumerable<ConfigurationEntry> Load()
        {
            if (!Directory.Exists(BaseFolder))
                return new ConfigurationEntry[0];

            Logger.Info("Scanning for configuration entries in '{0}'...", BaseFolder);
            var files = Directory.GetFiles(BaseFolder, "*." + CONFIG_FILE_EXTENSION, SearchOption.AllDirectories).ToList();

            var entries = new List<FileSystemConfigurationEntry>();
            entries.AddRange(files.Select(
                filename =>
                    {
                        try
                        {
                            return new FileSystemConfigurationEntry
                            {
                                Entry = Serialiser.FromJsonInFile<ConfigurationEntry>(filename),
                                FileInfo = new FileInfo(filename)
                            };
                        }
                        catch (Exception e)
                        {
                            var ex = new InvalidOperationException(string.Format("Processing '{0}'", filename), e);
                            Logger.Error(Logger.Event.During("FileSystemScheduleConfigurationRepository.Load()").Encountered(ex));
                            return null;
                        }
                    })
                    .Where(e => e != null));

            var validEntries = ProcessEntries(entries);
            return validEntries.Select(e => e.Entry);
        }

        protected static bool GetType<T>(string targetTypeName, out Type targetType)
        {
            targetType = null;
            Type[] matchingTypes;

            if (!TypeDiscovery.Discover<T>(t => t.Name.Equals(targetTypeName, StringComparison.OrdinalIgnoreCase), 
                out matchingTypes))
                return false;

            if (matchingTypes.Count() != 1)
                throw new InvalidOperationException(string.Format("Searching for type '{0}' named '{1}'; found {2} matches, expected only 1",
                    typeof(T).Name,
                    targetTypeName,
                    matchingTypes.Count()));

            targetType = matchingTypes.First();
            return true;
        }

        protected static void WriteConfigFile(ConfigurationEntry entry, string filepath)
        {
            string newname;

            if (!entry.RequiredProperties.TryGetValue(ConfigurationEntry.RequiredPropertyNames.NAME, out newname))
                throw new InvalidOperationException(string.Format("Unable to update configuration, '{0}' property is missing",
                    ConfigurationEntry.RequiredPropertyNames.NAME));

            var folder = Path.GetDirectoryName(filepath) ?? string.Empty;

            // detect a name change...
            if (!string.IsNullOrWhiteSpace(newname) &&
                !newname.Equals(entry.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                var ext = Path.GetExtension(filepath);
                var newfilepath = Path.Combine(folder, Path.ChangeExtension(newname, ext));

                if (File.Exists(newfilepath))
                    throw new InvalidOperationException(string.Format(
                        "Unable to rename configuration, entry with name '{0}' already exists in '{1}'", newname, folder));

                // update entry properties
                entry.Name = newname;

                // do the disk work...
                Serialiser.ToJsonInFile(newfilepath, entry);
                File.Delete(filepath);
            }
            else
            {
                // just save it
                Serialiser.ToJsonInFile(filepath, entry);               
            }
        }

        protected bool HandleChange(ConfigurationChangeRequest change, string filepath)
        {
            switch (change.Action.ToLowerInvariant())
            {
                case ConfigurationActions.Delete:
                    File.Delete(filepath);
                    return true;

                case ConfigurationActions.Update:
                    WriteConfigFile(change.Entry, filepath);
                    return true;
            }

            return false;
        }

        protected virtual void FindAndExecuteBootstrappers(Type configType, object config)
        {
            var bootstrapBuilderType = typeof(ISupportBootStrapping<>);
            var bootstrapperInterfaceType = bootstrapBuilderType.MakeGenericType(configType);

            Type[] bootstrapperTypes;
            if (!TypeDiscovery.Discover(bootstrapperInterfaceType, out bootstrapperTypes))
                return;

            // found some...
            foreach (var bootstrapperType in bootstrapperTypes)
            {
                dynamic bootstrapper = Activator.CreateInstance(bootstrapperType);
                bootstrapperInterfaceType.GetMethod("Execute")
                    .Invoke(bootstrapper, new[] { config });
            }
        }

        protected virtual bool IsDisabled(object config)
        {
            return (config is ICanBeSwitchedOff && !((ICanBeSwitchedOff)config).Enabled);
        }
    }
}