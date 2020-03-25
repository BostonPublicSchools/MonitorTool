using System;
using System.Collections.Generic;
using System.Linq;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;
using Castle.Core;

namespace Wolfpack.Core.Loaders
{
    public class HealthCheckLoader : ILoader<IHealthCheckSchedulerPlugin>
    {
        private readonly ILoader<BindingConfiguration> myBindingConfigLoader;

        public HealthCheckLoader(ILoader<BindingConfiguration> bindingConfigLoader)
        {
            myBindingConfigLoader = bindingConfigLoader;
        }

        public bool Load(out IHealthCheckSchedulerPlugin[] components)
        {
            components = new IHealthCheckSchedulerPlugin[0];

            Binding[] bindings;
            var bindingLoader = new HealthCheckBindingLoader(myBindingConfigLoader);
         
            if (!bindingLoader.Load(out bindings))
                return false;

            var activityPlugins = new List<IHealthCheckSchedulerPlugin>();

            bindings.ToList().ForEach(b =>
                                      {
                                          Logger.Debug("Binding: {0} to {1}", b.Check.GetType().Name,
                                              b.Scheduler.GetType().Name);

                                          activityPlugins.Add(b.Scheduler);
                                      });

            var plugins = activityPlugins.Cast<IPlugin>().InitialisePlugins();
            components = plugins.Cast<IHealthCheckSchedulerPlugin>().ToArray();
            return (components.Length > 0);
        }

        public bool Load(out IHealthCheckSchedulerPlugin[] components, Action<IHealthCheckSchedulerPlugin> action)
        {
            var result = Load(out components);
            components.ToList().ForEach(action);
            return result;
        }
    }
}