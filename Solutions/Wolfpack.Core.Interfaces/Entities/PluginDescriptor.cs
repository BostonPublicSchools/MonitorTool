using System;

namespace Wolfpack.Core.Interfaces.Entities
{
    /// <summary>
    /// This identifies and describes a health check
    /// </summary>
    public class PluginDescriptor
    {
        /// <summary>
        /// The unique id of this test
        /// </summary>
        public Guid TypeId { get; set; }

        /// <summary>
        /// Friendly name for display
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A description of the check
        /// </summary>
        public string Description { get; set; }
    }
}