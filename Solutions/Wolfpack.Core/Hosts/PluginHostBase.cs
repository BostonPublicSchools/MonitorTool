using System.Collections.Generic;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Hosts
{
    public abstract class PluginHostBase : IServicePlugin
    {
        protected List<IServicePlugin> _plugins = new List<IServicePlugin>();

        public abstract PluginDescriptor Identity { get; }

        public Status Status { get; set; }

        public void Initialise()
        {
            Logger.Debug("Initialising {0} plugins...", _plugins.Count);
            _plugins.ForEach(p =>
            {
                Logger.Debug("\t{0}", p.GetType().Name);
                p.Initialise();
            });
        }

        public virtual void Start()
        {
            Logger.Debug("Starting {0} plugins...", _plugins.Count);
            _plugins.ForEach(p =>
            {
                Logger.Debug("\t{0}", p.GetType().Name);
                p.Start();
            });
        }

        public virtual void Stop()
        {
            Logger.Debug("Stopping {0} plugins...", _plugins.Count);
            _plugins.ForEach(p =>
            {
                Logger.Debug("\t{0}", p.GetType().Name);
                p.Stop();
            });
        }

        public virtual void Pause()
        {
            Logger.Debug("Pausing plugins...");
            _plugins.ForEach(p =>
            {
                Logger.Debug("\t{0}", p.GetType().Name);
                p.Pause();
            });
        }

        public virtual void Continue()
        {
            Logger.Debug("Continuing plugins...");
            _plugins.ForEach(p =>
            {
                Logger.Debug("\t{0}", p.GetType().Name);
                p.Continue();
            });
        }
    }
}