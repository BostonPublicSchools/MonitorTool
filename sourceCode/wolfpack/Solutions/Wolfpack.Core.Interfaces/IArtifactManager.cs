using System;
using System.IO;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Interfaces
{
    public class ArtifactContentTypes
    {
        public const string CSV = "text/csv";
    }

    public interface IArtifactManager
    {
        Stream Get(string checkname, Guid notificationId);
        ArtifactDescriptor Save(PluginDescriptor owner, string contentType, object data);
        ArtifactDescriptor Save(PluginDescriptor owner, IArtifactFormatter formatter, object data);
        void Initialise();
    }
}