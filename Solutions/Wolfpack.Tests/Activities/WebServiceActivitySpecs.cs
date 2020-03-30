using System;
using NUnit.Framework;
using StoryQ;
using Wolfpack.Core.Testing.Bdd;
using Wolfpack.Core.WebServices.Interfaces.Exceptions;

namespace Wolfpack.Tests.Activities
{
    [TestFixture]
    public class WebServiceActivitySpecs : BddFeature
    {
        protected override Feature DescribeFeature()
        {
            return new Story("Exchanging data and processing commands with a Wolfpack instance via its REST api")
                .InOrderTo("Interact with a Wolfpack instance")
                .AsA("REST client")
                .IWant("the API to take care of this for me")
                .Tag("WebServices")
                .Tag("Api");
        }

        [Test]
        public void PingStatusPage()
        {
            using (var domain = new WebServicesActivityDomain())
            {
                Feature.WithScenario("calling the status api method")
                    .Tag("Status")
                    .Given(domain.TheDefaultConfigurationManagerIsLoaded)
                        .And(domain.TheWebServiceActivityIsLoaded)
                        .And(domain.TheAgentIsStarted)
                    .When(domain.TheWebServiceStatusApiIsCalled)
                    .Then(domain.TheWebServiceStatusShouldBeValid)
                        .And(domain.ThrewNoException)
                    .ExecuteWithReport();
            }
        }

        [Test]
        public void PingStatusPageWithMatchingApiKey()
        {
            using (var domain = new WebServicesActivityDomain())
            {
                Feature.WithScenario("calling the status api method with a matching apikey")
                    .Tag("Status")
                    .Given(domain.TheDefaultConfigurationManagerIsLoaded)
                        .And(domain.TheWebServiceClientApiKeyIs, "hello")
                        .And(domain.TheWebServiceActivityApiKeyIs, "hello")
                        .And(domain.TheWebServiceActivityIsLoaded)
                        .And(domain.TheAgentIsStarted)
                    .When(domain.TheWebServiceStatusApiIsCalled)
                    .Then(domain.ThrewNoException)
                        .And(domain.TheWebServiceStatusShouldBeValid)                        
                    .ExecuteWithReport();
            }
        }

        [Test]
        public void PingStatusPageWithNonMatchingApiKey()
        {
            using (var domain = new WebServicesActivityDomain())
            {
                Feature.WithScenario("calling the status api method with a mis-matched apikey")
                    .Tag("Status")
                    .Given(domain.TheDefaultConfigurationManagerIsLoaded)
                        .And(domain.TheWebServiceClientApiKeyIs, "hello")
                        .And(domain.TheWebServiceActivityApiKeyIs, "goodbye")
                        .And(domain.TheWebServiceActivityIsLoaded)
                        .And(domain.TheAgentIsStarted)
                    .When(domain.TheWebServiceStatusApiIsCalled)
                    .Then(domain.ShouldThrow_Exception, typeof(CommunicationException))
                    .ExecuteWithReport();
            }
        }

        [Test]
        public void GetCatalogueWithNoTags()
        {
            using (var domain = new WebServicesActivityDomain())
            {
                Feature.WithScenario("get the catalogue with no tags")
                    .Tag("Catalogue")
                    .Given(domain.TheDefaultConfigurationManagerIsLoaded)
                        .And(domain.TheWebServiceActivityIsLoaded)
                        .And(domain.TheAgentIsStarted)
                    .When(domain.TheWebServiceGetCatalogueApiIsCalled)
                    .Then(domain.TheWebServiceCatalogueShouldBeValid)
                    .ExecuteWithReport();
            }
        }

        [Test]
        public void GetCatalogueWithTags()
        {
            using (var domain = new WebServicesActivityDomain())
            {
                Feature.WithScenario("get the catalogue with tags")
                    .Tag("Catalogue")
                    .Given(domain.TheDefaultConfigurationManagerIsLoaded)
                        .And(domain.TheWebServiceActivityIsLoaded)
                        .And(domain.TheAgentIsStarted)
                    .When(domain.TheWebServiceGetCatalogueApiIsCalledWithTags_, "Test")
                    .Then(domain.TheWebServiceCatalogueShouldBeValid)
                        .And(domain.TheWebServiceCatalogueShouldContain_Items, 1)
                    .ExecuteWithReport();
            }
        }

        [Test]
        public void NotificationPublishedToTheWebService()
        {
            using (var domain = new WebServicesActivityDomain())
            {
                var id = Guid.NewGuid();

                Feature.WithScenario("publishing a message")
                    .Tag("Publishing")
                    .Given(domain.TheDefaultConfigurationManagerIsLoaded)
                        .And(domain.TheWebServiceActivityIsLoaded)
                        .And(domain.TheWebServicePublisherClientActivityIsLoaded)
                        .And(domain.TheAgentIsStarted)
                    .When(domain.ANotificationIsSentWithId_, id)
                    .Then(domain.TheWebServiceShouldHaveReceivedMessage_, id)
                    .ExecuteWithReport();
            }
        }
    }
}