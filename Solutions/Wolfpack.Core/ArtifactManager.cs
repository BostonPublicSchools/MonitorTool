using System;
using System.IO;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core
{
    public static class ArtifactManager
    {
        public class KnownContentTypes
        {
            public const string TabSeparated = "text/csv";
            public const string Json = "application/json";
        }

        private static IArtifactManager _instance;

        public static void Initialise(IArtifactManager instance)
        {
            _instance = instance;
            _instance.Initialise();
        }

        public static Stream Get(string checkname, Guid notificationId)
        {
            return _instance.Get(checkname, notificationId);
        }

        public static ArtifactDescriptor Save(PluginDescriptor owner, string contentType, object data)
        {
            return _instance.Save(owner, contentType, data);
        }

        public static ArtifactDescriptor Save(PluginDescriptor owner, IArtifactFormatter formatter, object data)
        {
            return _instance.Save(owner, formatter, data);
        }
    }
}