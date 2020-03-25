using System;
using System.Collections.Generic;
using System.IO;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Artifacts
{
    public class FileSystemArtifactManager : IArtifactManager
    {
        public const string DefaultExtension = "dat";

        private readonly string _baseFolder;
        private readonly IEnumerable<IArtifactFormatter> _formatters; 

        public FileSystemArtifactManager(string baseFolder, IEnumerable<IArtifactFormatter> formatters)
        {
            _baseFolder = SmartLocation.GetLocation(baseFolder);
            _formatters = formatters;
        }

        public Stream Get(string checkname, Guid notificationId)
        {
            var folder = Path.Combine(_baseFolder, checkname);
            var filepath = Path.Combine(folder, string.Format("{0}.{1}", notificationId, DefaultExtension));

            return new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public ArtifactDescriptor Save(PluginDescriptor owner, string contentType, object data)
        {
            var folder = Path.Combine(_baseFolder, owner.Name);

            var descriptor = new ArtifactDescriptor
            {
                Location = folder,
                Owner = owner
            };

            Directory.CreateDirectory(folder);

            var formatter = _formatters.Find(contentType);
            var filepath = Path.Combine(folder, Path.ChangeExtension(descriptor.Id.ToString(), DefaultExtension));

            using (var sw = new FileStream(filepath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (var datastream = formatter.Serialize(data))
                {
                    datastream.ChunkedOperation(4000, (buffer, size) =>
                                                          {
                                                              sw.Write(buffer, 0, size);
                                                          });
                }                
            }

            return descriptor;
        }

        public ArtifactDescriptor Save(PluginDescriptor owner, IArtifactFormatter formatter, object data)
        {
            throw new System.NotImplementedException();
        }

        public void Initialise()
        {
            Directory.CreateDirectory(_baseFolder);
        }
    }
}