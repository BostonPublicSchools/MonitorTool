using System.IO;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Configuration.FileSystem
{
    public class FileSystemConfigurationEntry
    {
        public FileInfo FileInfo { get; set; }
        public ConfigurationEntry Entry { get; set; }
    }
}