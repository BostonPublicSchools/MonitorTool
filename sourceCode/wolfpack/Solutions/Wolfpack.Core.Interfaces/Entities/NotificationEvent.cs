using System;
using System.Collections.Generic;
using System.Linq;

namespace Wolfpack.Core.Interfaces.Entities
{
    public class NotificationEvent : INotificationEventCore
    {
        public Guid Id { get; set; }
        public string EventType { get; set; }
        public string SiteId { get; set; }
        public string AgentId { get; set; }
        public string CheckId { get; set; }
        public string Message { get; set; }        
        public bool CriticalFailure { get; set; }
        public CriticalFailureDetails CriticalFailureDetails { get; set; }
        public bool? Result { get; set; }
        public double? ResultCount { get; set; }
        public string DisplayUnit { get; set; }
        public DateTime GeneratedOnUtc { get; set; }
        public DateTime? ReceivedOnUtc { get; set; }
        public List<string> Tags { get; set; }
        public int? HourBucket { get; set; }
        public int? MinuteBucket { get; set; }
        public int? DayBucket { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Data { get; set; }
        public Guid Version { get; set; }
        public Properties Properties { get; set; }
        public MessageStateTypes State { get; set; }

        public NotificationEvent()
        {
            Id = Guid.NewGuid();
            GeneratedOnUtc = DateTime.UtcNow;
            Properties = new Properties();
            Tags = new List<string>();
        }

        public void AssociateArtifact(ArtifactDescriptor artifact)
        {
            if (artifact != null)
                Id = artifact.Id;
        }

        public IDictionary<string, object> ToDictionary()
        {
            var result = GetType().GetProperties()
                .Where(pi => !pi.Name.Equals("Properties") &&
                    !pi.Name.Equals("CriticalFailureDetails"))
                .ToDictionary(pi => pi.Name, pi => pi.GetValue(this, null),
                    StringComparer.OrdinalIgnoreCase
                );

            if (Properties != null)
            {
                foreach (var property in Properties)
                    result.Add(string.Format("Property_{0}", property), property.Value);
            }

            if (CriticalFailureDetails != null)
            {
                foreach (var pi in typeof(CriticalFailureDetails).GetProperties())
                {
                    result.Add(string.Format("CriticalFailureDetails_{0}", pi.Name), 
                        pi.GetValue(CriticalFailureDetails, null));
                }                
            }

            return result;
        }
    }
}