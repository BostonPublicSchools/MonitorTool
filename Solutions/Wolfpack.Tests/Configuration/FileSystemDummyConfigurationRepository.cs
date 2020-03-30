using System.Collections.Generic;
using Wolfpack.Core.Configuration.FileSystem;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Tests.Configuration
{
    public class FileSystemDummyConfigurationRepository : FileSystemConfigurationRepository
    {
        public FileSystemDummyConfigurationRepository(string baseFolder) : base(baseFolder)
        {
        }

        protected override IEnumerable<FileSystemConfigurationEntry> ProcessEntries(ICollection<FileSystemConfigurationEntry> entries)
        {
            return entries;
        }

        public override void Save(ConfigurationChangeRequest change)
        {
            throw new global::System.NotImplementedException();
        }
    }
}