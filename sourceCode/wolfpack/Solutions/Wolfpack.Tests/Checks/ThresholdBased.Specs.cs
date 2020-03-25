using NUnit.Framework;
using StoryQ;
using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.Notification.Filters.Request;
using Wolfpack.Core.Testing.Bdd;
using Wolfpack.Core.Testing.Domains;

namespace Wolfpack.Tests.Checks
{
    [TestFixture]
    public class ThresholdBasedSpecs : BddFeature
    {
        protected override Feature DescribeFeature()
        {
            return new Story("Be able to test health checks that utilise a threshold")
                .InOrderTo("Capture data and receive alerts")
                .AsA("user")
                .IWant("The component to behave correctly");
        }

        [Test]
        public void NoThresholdFailureOnly()
        {
            using (var domain = new ThresholdCheckDomain(ThresholdCheckDomainConfig.FiresValues(1)
                .NotificationModeIs(FailureOnlyNotificationFilter.FilterName)))
            {
                Feature.WithScenario("check failures only and no threshold set")
                    .Given(domain.TheDefaultNotificationFiltersAreLoaded)
                        .And(domain.TheAgentIsStarted)
                    .When(domain.TheHealthCheckIsInvoked)
                    .Then(domain.ThereShouldBe_NotificationRequestMessagesOfType_Published, 1, NotificationEventHealthCheck.EventTypeName)
                        .And(domain.ThereShouldBe_HealthCheckNotificationsReceived, 1)
                    .ExecuteWithReport();
            }
        }
        
        
        [Test]
        public void AlwaysPublishNoAlertThreshold()
        {
            using (var domain = new ThresholdCheckDomain(ThresholdCheckDomainConfig.FiresValues(1)))
            {
                Feature.WithScenario("Always publish and no alert threshold set")
                    .Given(domain.TheDefaultNotificationFiltersAreLoaded)
                        .And(domain.TheAgentIsStarted)
                    .When(domain.TheHealthCheckIsInvoked)
                    .Then(domain.ThereShouldBe_NotificationRequestMessagesOfType_Published, 1, NotificationEventHealthCheck.EventTypeName)
                        .And(domain.ThereShouldBe_HealthCheckNotificationsReceived, 1)
                    .ExecuteWithReport();
            }
        }
         
        [Test]
        public void FailureOnlyAlertThresholdSetNoTrigger()
        {
            using (var domain = new ThresholdCheckDomain(ThresholdCheckDomainConfig.FiresValues(1)
                .NotificationModeIs(FailureOnlyNotificationFilter.FilterName)
                .ThresholdIs(10)))
            {
                Feature.WithScenario("Failure Only and an alert threshold has been set but not triggered")
                    .Given(domain.TheDefaultNotificationFiltersAreLoaded)
                        .And(domain.TheAgentIsStarted)
                    .When(domain.TheHealthCheckIsInvoked)
                    .Then(domain.ThereShouldBe_NotificationRequestMessagesOfType_Published, 1, NotificationEventHealthCheck.EventTypeName)
                        .And(domain.TheNotificationRequestAtIndex_ShouldHaveResult_, 1, true)
                        .And(domain.ThereShouldBe_HealthCheckNotificationsReceived, 0)
                    .ExecuteWithReport();
            }
        }        

        [Test]
        public void FailureOnlyNotificationModeWithLowAndHighAlertsGenerated()
        {
            using (var domain = new ThresholdCheckDomain(ThresholdCheckDomainConfig.FiresValues(1,11)
                .ThresholdIs(10)
                .CheckNameIs("bob")
                .NotificationModeIs(FailureOnlyNotificationFilter.FilterName)))
            {
                Feature.WithScenario("FailureOnly NotificationMode and an alert threshold has been set with a low and a high value received")
                    .Given(domain.TheDefaultNotificationFiltersAreLoaded)
                        .And(domain.TheAgentIsStarted)
                    .When(domain.TheHealthCheckIsInvoked)
                    .Then(domain.ThereShouldBe_NotificationRequestMessagesOfType_Published, 2, NotificationEventHealthCheck.EventTypeName)
                        .And(domain.ThereShouldBe_HealthCheckNotificationsReceived, 1)
                        .And(domain.TheNotificationRecievedAtIndex_ShouldHaveResult_, 1, false)
                        .And(domain.TheNotificationReceivedAtIndex_ShouldHaveResultCount_, 1, 11d)
                    .ExecuteWithReport();
            }
        }     
    }
}