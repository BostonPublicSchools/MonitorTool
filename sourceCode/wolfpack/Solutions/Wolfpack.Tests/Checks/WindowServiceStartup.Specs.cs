using System;
using System.Collections.Generic;
using Wolfpack.Core.Checks;
using NUnit.Framework;
using StoryQ;
using Wolfpack.Core.Testing.Bdd;

namespace Wolfpack.Tests.Checks
{
    [TestFixture]
    public class WindowServiceStartupSpecs : BddFeature
    {
        [Test]
        public void ValidServiceCorrectStartup()
        {
            using (var domain = new WindowServiceStartupDomain(new WindowsServiceStartupCheckConfig
                                                  {
                                                      Enabled = true,
                                                      ExpectedStartupType = "Auto",
                                                      FriendlyId = "HealthCheckTest",
                                                      Services = new List<string>
                                                                     {
                                                                         "DHCP Client"
                                                                     }
                                                  }))
            {
                Feature.WithScenario("A valid windows service name specified and it should have the expected startup type")
                    .Given(domain.TheAgentIsStarted)
                    .When(domain.TheHealthCheckIsInvoked)
                    .Then(domain.ThrewNoException)
                        .And(domain.ThereShouldBe_HealthCheckNotificationsReceived, 0)
                    .ExecuteWithReport();
            }
        }

        [Test]
        public void ValidServiceIncorrectStartup()
        {
            using (var domain = new WindowServiceStartupDomain(new WindowsServiceStartupCheckConfig
                                                  {
                                                      Enabled = true,
                                                      ExpectedStartupType = "Disabled",
                                                      FriendlyId = "HealthCheckTest",
                                                      Services = new List<string>
                                                                     {
                                                                         "DHCP Client"
                                                                     }
                                                  }))
            {
                Feature.WithScenario("A valid windows service name specified but the startup type is incorrect")
                    .Given(domain.TheAgentIsStarted)
                    .When(domain.TheHealthCheckIsInvoked)
                    .Then(domain.ThereShouldBe_HealthCheckNotificationsReceived, 1)
                    //.And(domain.TheResultMessageShouldIndicateFailure)
                    .ExecuteWithReport();
            }
        }

        [Test]
        public void InvalidService()
        {
            const string serviceName = "ASDEDFG123234fwerw4f";

            using (var domain = new WindowServiceStartupDomain(new WindowsServiceStartupCheckConfig
                                                  {
                                                      Enabled = true,
                                                      ExpectedStartupType = "Auto",
                                                      FriendlyId = "HealthCheckTest",
                                                      Services = new List<string>
                                                                     {
                                                                         serviceName
                                                                     }
                                                  }))
            {
                Feature.WithScenario("An invalid windows service name specified")
                    .Given(domain.TheAgentIsStarted)
                    .When(domain.TheHealthCheckIsInvoked)
                    .Then(domain.ShouldThrow_Exception, typeof(InvalidOperationException))
                        .And(domain._ShouldBeInTheExceptionMesssage, serviceName)
                    .ExecuteWithReport();
            }
        }

        [Test]
        public void MultipleInvalidServices()
        {
            const string serviceName1 = "ASDEDFG123234fwerw4f";
            const string serviceName2 = "ERseSfsdfDF23£23eE";

            using (var domain = new WindowServiceStartupDomain(new WindowsServiceStartupCheckConfig
                                                  {
                                                      Enabled = true,
                                                      ExpectedStartupType = "Auto",
                                                      FriendlyId = "HealthCheckTest",
                                                      Services = new List<string>
                                                                     {
                                                                         serviceName1,
                                                                         serviceName2
                                                                     }
                                                  }))
            {

                Feature.WithScenario("Multiple invalid windows service names supplied")
                    .Given(domain.TheAgentIsStarted)
                    .When(domain.TheHealthCheckIsInvoked)
                    .Then(domain.ShouldThrow_Exception, typeof(InvalidOperationException))
                        .And(domain._ShouldBeInTheExceptionMesssage, serviceName1)
                        .And(domain._ShouldBeInTheExceptionMesssage, serviceName2)
                    .ExecuteWithReport();
            }
        }

        protected override Feature DescribeFeature()
        {
            return new Story("The ensure the HealthCheck behaves as expected")
                .Tag("HealthCheck")
                .Tag("Windows Service")
                .InOrderTo("Ensure an exception is raised")
                .AsA("Wolfpack user")
                .IWant("The correct behaviour");
        }
    }
}