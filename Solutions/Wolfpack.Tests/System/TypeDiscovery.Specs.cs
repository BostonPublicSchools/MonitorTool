using NUnit.Framework;
using StoryQ;
using Wolfpack.Agent.Profiles;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Testing.Bdd;
using Wolfpack.Core.WebServices.Interfaces;
using Wolfpack.Core.WebServices.Interfaces.Entities;
using Wolfpack.Core.WebServices.Strategies.Steps;

namespace Wolfpack.Tests.System
{
    public class TypeDiscoverySpecs : BddFeature
    {
        protected override Feature DescribeFeature()
        {
            return new Story("Testing Wolfpack TypeDiscovery")
                .InOrderTo("load implementations")
                .AsA("consumer of components")
                .IWant("the Typediscovery system to take care of this");
        }

        [Test]
        public void DiscoverGenericType()
        {
            using (var domain = new TypeDiscoveryDomain())
            {
                Feature.WithScenario("Discovery a generic interface type")
                    .Given(domain.ThisType_ToDiscover, typeof(IPipelineStep<WebServicePublisherContext>))
                    .When(domain.TheTypeDiscoveryIsExecuted)
                    .Then(domain.This_TypeShouldBeDiscovered, typeof(SendMessagesStep))
                    .ExecuteWithReport();
            }
        }

        [Test]
        public void DiscoverSimpleType()
        {
            using (var domain = new TypeDiscoveryDomain())
            {
                Feature.WithScenario("Discovery a simple interface type")
                    .Given(domain.ThisType_ToDiscover, typeof(IRoleProfile))
                    .When(domain.TheTypeDiscoveryIsExecuted)
                    .Then(domain.This_TypeShouldBeDiscovered, typeof(DefaultAgentProfile))
                    .ExecuteWithReport();
            }
        }
    }
}