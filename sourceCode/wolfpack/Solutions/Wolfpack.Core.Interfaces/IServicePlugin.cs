using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Interfaces
{
    public interface IServicePlugin : IPlugin
    {
        /// <summary>
        /// Used to identify a plugin
        /// </summary>
        PluginDescriptor Identity { get; }

        void Start();
        void Stop();
        void Pause();
        void Continue();
    }
}