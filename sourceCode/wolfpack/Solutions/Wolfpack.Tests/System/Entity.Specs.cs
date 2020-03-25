using NUnit.Framework;
using StoryQ;
using Wolfpack.Core.Testing.Bdd;

namespace Wolfpack.Tests.System
{
    public class EntitySpecs : BddFeature
    {
        protected override Feature DescribeFeature()
        {
            return new Story("Testing Wolfpack data entities")
                .InOrderTo("not break data entities")
                .AsA("component that interacts with the entities")
                .IWant("these tests to prove this is so!");
        }

        [TestCase("v2.4.0")]
        [Ignore]
        public void HealthCheckResultDeserialisePreviousVersion(string version)
        {
            using (var domain = new EntitiesDomain())
            {
                Feature.WithScenario("")
                    .Given(domain.TheHealthCheckResultDataFilenameIsBuiltForVersion_, version)
                    .When(domain.TheHealthCheckResultXmlIsDeserialised)
                    .Then(domain.ThrewNoException)
                        .And(domain.TheResultShouldNotBeNull)
                    .ExecuteWithReport();
            }
        }

        [Test]
        public void AllNotificationPropertiesAreCopiedFromRequestToEvent()
        {
            using (var domain = new EntitiesDomain())
            {
                Feature.WithScenario("when converting a notification request to an event all notification properties are copied across")
                    .Given(domain.TheNotificationRequestIsFullyPopulated)
                    .When(domain.TheNotificationRequestIsConvertedToAnEvent)
                    .Then(domain.AllTheNotificationEventPropertiesMatchTheRequestNotification)
                    .ExecuteWithReport();
            }
        }
    }
}