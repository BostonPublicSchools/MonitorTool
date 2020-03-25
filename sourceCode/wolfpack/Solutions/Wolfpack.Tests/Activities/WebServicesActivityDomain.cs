using System;
using Magnum.Pipeline;
using NUnit.Framework;
using Wolfpack.Core;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.Testing.Domains;
using Wolfpack.Core.Testing.Drivers;
using Wolfpack.Core.WebServices;
using Wolfpack.Core.WebServices.Client;
using Wolfpack.Core.WebServices.Interfaces;
using Wolfpack.Core.WebServices.Interfaces.Entities;
using Wolfpack.Core.WebServices.Interfaces.Messages;
using Wolfpack.Core.WebServices.Publisher;

namespace Wolfpack.Tests.Activities
{
    public class WebServicesActivityDomain : SystemDomainBase
    {
        public const string BaseUrl = "http://localhost:802/wolfpack";

        private StatusResponse _status;
        private RestConfigurationCatalogue _catalogue;
        private string _clientApiKey;
        private string _serverApiKey;

        public void TheWebServiceActivityIsLoaded()
        {
            var config = new WebServiceActivityConfig
                             {
                                 BaseUrl = BaseUrl,
                                 Enabled = true
                             };

            if (!string.IsNullOrWhiteSpace(_serverApiKey))
                config.ApiKeys.Add(_serverApiKey);

            Agent.Run(new WebServiceTestBootstrapper(config));
            Agent.Run(agent =>
                          {
                              var service = Container.Resolve<IActivityPlugin, WebServiceActivity>();
                              agent.Run(service);
                          });

        }

        public void TheWebServiceStatusApiIsCalled()
        {
            SafeExecute(() => _status = RestClient.GetStatus());
        }

        public void TheWebServiceStatusShouldBeValid()
        {
            Assert.That(_status, Is.Not.Null);
        }

        private IWolfpackWebServicesClient RestClient
        {
            get
            {
                var config = new WolfpackWebServicesClientConfig
                                 {
                                     BaseUrl = BaseUrl,
                                     ApiKey = _clientApiKey

                                 };
                return new WolfpackWebServicesClient(config);
            }
        }

        public void TheWebServiceGetCatalogueApiIsCalled()
        {
            _catalogue = RestClient.GetCatalogue(new GetConfigurationCatalogue());
        }

        public void TheWebServiceGetCatalogueApiIsCalledWithTags_(string csvTags)
        {
            var tags = csvTags.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
            _catalogue = RestClient.GetCatalogue(new GetConfigurationCatalogue { Tags = tags });
        }

        public void TheWebServiceCatalogueShouldBeValid()
        {
            Assert.That(_catalogue, Is.Not.Null);
        }

        public void TheWebServiceCatalogueShouldContain_Items(int expected)
        {
            Assert.That(_catalogue.Items.Count, Is.EqualTo(expected));
        }

        public void TheWebServicePublisherClientActivityIsLoaded()
        {
            var config = new WebServicePublisherConfig
                             {
                                 Enabled = true,
                                 BaseUrl = BaseUrl,
                                 BaseFolder = "_outbox",
                                 ApiKey = _clientApiKey
                             };

            Agent.Run(new BootstrapperRunner<WebServicePublisherConfig, WebServicePublisherBootstrapper>(config));
            Agent.Run(agent =>
                          {
                              // iactivityplugins are usually autoregistered by infrastructure
                              Container.RegisterAsSingleton<IActivityPlugin>(typeof(WebServicePublisher));

                              var publisher = Container.Resolve<IActivityPlugin, WebServicePublisher>();
                              agent.Run(publisher);
                          });
        }

        public void ANotificationIsSentWithId_(Guid id)
        {
            var publisher = (IConsumer<NotificationEvent>) Container.Resolve<IActivityPlugin, WebServicePublisher>();
            publisher.Consume(new NotificationEvent
                                  {
                                      Id = id
                                  });
        }

        public void TheWebServiceShouldHaveReceivedMessage_(Guid expectedId)
        {
            Agent.WaitUntil("Waiting for notification to arrive...", 10, 
                            () =>
                                {
                                    var receiver = (WebServiceTestReceiverStrategy)Container.Resolve<IWebServiceReceiverStrategy>();
                                    return receiver.NotificationsReceived.ContainsKey(expectedId);
                                });
        }

        public void TheWebServiceClientApiKeyIs(string apikey)
        {
            _clientApiKey = apikey;
        }

        public void TheWebServiceActivityApiKeyIs(string apikey)
        {
            _serverApiKey = apikey;
        }
    }
}