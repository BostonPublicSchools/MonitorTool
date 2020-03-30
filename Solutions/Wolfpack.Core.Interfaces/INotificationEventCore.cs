using System;
using System.Collections.Generic;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Interfaces
{
    public interface INotificationEventCore
    {
        Guid Id { get; set; }
        string EventType { get; set; }
        string SiteId { get; set; }
        string AgentId { get; set; }
        string CheckId { get; set; }
        bool? Result { get; set; }
        double? ResultCount { get; set; }
        string DisplayUnit { get; set; }
        DateTime GeneratedOnUtc { get; set; }
        DateTime? ReceivedOnUtc { get; set; }
        Guid Version { get; set; }
        List<string> Tags { get; set; }
        int? HourBucket { get; set; }
        int? MinuteBucket { get; set; }
        int? DayBucket { get; set; }
        string Latitude { get; set; }
        string Longitude { get; set; }
        Properties Properties { get; set; }

        Guid TypeId { get; set; }
        // v3 new 
        bool CriticalFailure { get; set; }
        CriticalFailureDetails CriticalFailureDetails { get; set; }
        string Message { get; set; }
        MessageStateTypes State { get; set; }

        void AssociateArtifact(ArtifactDescriptor artifact);
    }
}