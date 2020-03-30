using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Repositories.FileSystem;
using Wolfpack.Core.WebServices.Extenders;
using Wolfpack.Core.WebServices.Interfaces;
using Wolfpack.Core.WebServices.Interfaces.Entities;
using Wolfpack.Core.WebServices.Strategies;
using Wolfpack.Core.WebServices.Strategies.Steps;

namespace Wolfpack.Core.WebServices
{
    public class WebServiceBootstrapper : ISupportBootStrapping<WebServiceActivityConfig>
    {
        public void Execute(WebServiceActivityConfig config)
        {
            var tracker = new ActivityTracker();
            Container.RegisterInstance(tracker);
            Container.RegisterAsSingleton<IWebServiceExtender>(typeof(CoreServicesExtender));

            if (!Container.IsRegistered<IWebServiceReceiverStrategy>())
            {
                if (!Container.IsRegistered<INotificationRepository>())
                {
                    var repoConfig = new FileSystemNotificationRepositoryConfig
                    {
                        BaseFolder = SmartLocation.GetLocation("_notifications")
                    };
                    Container.RegisterInstance(repoConfig);
                    Container.RegisterAsTransient<INotificationRepository>(
                        typeof(FileSystemNotificationRepository));
                }

                if (!Container.IsRegistered<MessageStalenessCheckConfig>())
                    Container.RegisterAsSingleton(typeof(MessageStalenessCheckConfig));

                Container.RegisterAll<IPipelineStep<WebServiceReceiverContext>>();
                Container.RegisterAsTransient<IWebServiceReceiverStrategy>(typeof(WebServiceReceiverStrategy));
            }
        }
    }
}