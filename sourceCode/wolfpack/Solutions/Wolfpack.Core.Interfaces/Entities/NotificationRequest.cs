using System;
using System.Diagnostics;

namespace Wolfpack.Core.Interfaces.Entities
{
    [DebuggerDisplay("{CheckId}")]
    public class NotificationRequest
    {
        public string Mode { get; set; }
        public Func<INotificationEventCore, string> DataKeyGenerator { get; set; }
        public INotificationEventCore Notification { get; set; }
        public ArtifactDescriptor Artifact { get; set; }

        public string CheckId
        {
            get
            {
                return Notification == null ? "[Unknown]" : Notification.CheckId;
            }
        }

        public NotificationRequest AssociateArtifact(ArtifactDescriptor artifactDescriptor)
        {
            Artifact = artifactDescriptor;
            return this;
        }
    }
}