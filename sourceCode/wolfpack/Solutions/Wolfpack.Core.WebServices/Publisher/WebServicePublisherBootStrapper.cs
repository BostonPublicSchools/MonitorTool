using Wolfpack.Core.Containers;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Repositories.FileSystem;
using Wolfpack.Core.WebServices.Client;
using Wolfpack.Core.WebServices.Interfaces;
using Wolfpack.Core.WebServices.Interfaces.Entities;
using Wolfpack.Core.WebServices.Strategies;
using Omu.ValueInjecter;

namespace Wolfpack.Core.WebServices.Publisher
{
    public class WebServicePublisherBootstrapper : ISupportBootStrapping<WebServicePublisherConfig>
    {
        public void Execute(WebServicePublisherConfig config)
        {
            Container.RegisterInstance(config, If<WebServicePublisherConfig>.IsNotRegistered);

            if (!Container.IsRegistered<INotificationRepository>())
            {
                var repoConfig = new FileSystemNotificationRepositoryConfig
                {
                    BaseFolder = config.BaseFolder
                };
                Container.RegisterInstance(repoConfig);
                Container.RegisterAsTransient<INotificationRepository>(
                    typeof(FileSystemNotificationRepository));
            }

            if (!Container.IsRegistered<IWebServicePublisherStrategy>())
            {
                var clientConfig = new WolfpackWebServicesClientConfig();
                clientConfig.InjectFrom<LoopValueInjection>(config);
                Container.RegisterInstance(clientConfig);

                Container.RegisterAsTransient<IWolfpackWebServicesClient>(typeof(WolfpackWebServicesClient));
                Container.RegisterAll<IPipelineStep<WebServicePublisherContext>>();
                Container.RegisterAsTransient<IWebServicePublisherStrategy>(
                    typeof(WebServicePublisherStrategy));
            }
        }
    }
}