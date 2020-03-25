using System;

namespace Wolfpack.Core.Interfaces.Entities
{
    public class ArtifactDescriptor
    {
        public Guid Id { get; set; }
        public string Location { get; set; }
        public string ContentType { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public PluginDescriptor Owner { get; set; }

        public ArtifactDescriptor()
        {
            Id = Guid.NewGuid();
            CreatedOnUtc = DateTime.UtcNow;
        }
    }
}