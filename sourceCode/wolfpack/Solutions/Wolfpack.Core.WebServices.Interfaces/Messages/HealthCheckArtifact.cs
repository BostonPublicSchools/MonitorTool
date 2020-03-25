using System;

namespace Wolfpack.Core.WebServices.Interfaces.Messages
{
    public class HealthCheckArtifact
    {
        public string Name { get; set; }
        public Guid NotificationId { get; set; }
        public string ContentType { get; set; }
    }
}