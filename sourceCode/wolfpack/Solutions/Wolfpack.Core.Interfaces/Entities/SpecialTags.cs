using System.Collections.Generic;

namespace Wolfpack.Core.Interfaces.Entities
{
    public class SpecialTags
    {
        public const string RUNNING = "Running";
        public const string ACTIVITY = "Activity";
        public const string BOOTSTRAP = "Bootstrap";
        public const string HEALTH_CHECK = "HealthCheck";
        public const string PUBLISHER = "Publisher";
        public const string SCHEDULE = "Schedule";

        private static readonly List<string> NotForPersisting = new List<string>
                                                            {
                                                                RUNNING
                                                            };

        public static bool ThatShouldNotBePersisted(string tag)
        {
            return NotForPersisting.Contains(tag);
        }
    }
}