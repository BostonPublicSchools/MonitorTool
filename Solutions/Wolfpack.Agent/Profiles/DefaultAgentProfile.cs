using System;
using Wolfpack.Core;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Magnum;
using Wolfpack.Core.Loaders;

namespace Wolfpack.Agent.Profiles
{
    /// <summary>
    /// 
    /// </summary>
    public class DefaultAgentProfile : ProfileBase
    {
        public override string Name
        {
            get { return "DefaultAgent"; }
        }

        public virtual Type DefineRole()
        {
            return typeof (Roles.Agent);
        }

        public override void CustomiseRole()
        {            
            Container.RegisterAsSingleton<ILoader<IHealthCheckSchedulerPlugin>>(typeof(ContainerPluginLoader<IHealthCheckSchedulerPlugin>))
                .RegisterAsSingleton<ILoader<IActivityPlugin>>(typeof(ContainerPluginLoader<IActivityPlugin>))
                .RegisterAsSingleton<ILoader<INotificationEventPublisher>>(typeof(ContainerPluginLoader<INotificationEventPublisher>))
                .RegisterAsSingleton<IRolePlugin>(DefineRole());
        }
    }
}