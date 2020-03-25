using Wolfpack.Core;
using Wolfpack.Core.Configuration;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Notification;

namespace Wolfpack.Agent.Profiles
{
    /// <summary>
    /// Provides a common profile base functionality - this primarily
    /// consists of help constructing and registering publishers into
    /// the IoC container
    /// </summary>
    public abstract class ProfileBase : IRoleProfile
    {
        public abstract string Name { get; }
        public abstract void CustomiseRole();

        public IRolePlugin Role
        {
            get
            {
                // load and execute all startup plugins
                // ...not using .SafeInitialise() method as
                // we want this to blow up loading the agent
                Container.RegisterAll<IStartupPlugin>()
                    .ResolveAll<IStartupPlugin>(c => c.InitialiseIfEnabled());

                ConfigurationManager.Initialise();
                Messenger.Initialise();
                NotificationHub.Initialise();

                // hook to create custom role components
                CustomiseRole();

                // finally resolve the role component
                return Container.Resolve<IRolePlugin>();
            }
        }
    }
}