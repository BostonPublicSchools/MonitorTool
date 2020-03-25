using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Interfaces
{
    public interface IHealthCheckPlugin : IPlugin
    {
        /// <summary>
        /// Used to identify a plugin
        /// </summary>
        PluginDescriptor Identity { get; }

        /// <summary>
        /// This is the health check code - it is triggered by the 
        /// scheduler that this check has been bound to
        /// </summary>
        void Execute();
    }
}