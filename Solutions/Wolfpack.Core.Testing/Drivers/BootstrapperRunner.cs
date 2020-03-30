using System;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Testing.Drivers
{
    public class BootstrapperRunner<TConfig, TBootstrapper> : IStartupPlugin
        where TBootstrapper : ISupportBootStrapping<TConfig>
    {
        private readonly TConfig _config;

        public BootstrapperRunner(TConfig config)
        {
            _config = config;
        }

        public Status Status { get; set; }

        public void Initialise()
        {
            var bootstrapper = Activator.CreateInstance<TBootstrapper>();
            bootstrapper.Execute(_config);
        }
    }
}