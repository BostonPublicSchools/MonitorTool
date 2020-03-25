using NUnit.Framework;
using StoryQ;
using Wolfpack.Core.Checks;
using Wolfpack.Core.Testing.Bdd;

namespace Wolfpack.Tests.Checks
{
    [TestFixture]
    public class SqlQueryCheckSpecs : BddFeature
    {
        protected override Feature DescribeFeature()
        {
            return new Story("Sql Query Check performs as expected")
                .InOrderTo("run query based checks against a sql database")
                .AsA("sys admin")
                .IWant("the component to work correctly!")
                .Tag("SqlQueryCheck");
        }

        [Test]
        public void SqlQueryHappyPath()
        {
            using (var domain = new SqlQueryCheckDomain(new SqlQueryCheckConfig
                                               {
                                                   ConnectionString = "WolfpackR",
                                                   Query = "SELECT * FROM master.sys.databases"
                                               }))
            {
                Feature.WithScenario("returns some rows")
                    .Given(domain.TheAgentIsStarted)
                    .When(domain.TheHealthCheckIsInvoked)
                    .Then(domain.ThereShouldBe_HealthCheckNotificationsReceived, 1)
                        .And(domain.TheNotificationRecievedAtIndex_ShouldHaveResult_, 1, false)
                    .ExecuteWithReport();
            }
        }

        [Test]
        public void SqlQueryReturnsNoRows()
        {
            using (var domain = new SqlQueryCheckDomain(new SqlQueryCheckConfig
                                               {                                                
                                                   ConnectionString = "WolfpackR",
                                                   Query = "SELECT * FROM master.sys.databases WHERE 1=2"
                                               }))
            {
                Feature.WithScenario("returns no rows should create a successful result")
                    .Given(domain.TheAgentIsStarted)
                    .When(domain.TheHealthCheckIsInvoked)
                    .Then(domain.ThereShouldBe_HealthCheckNotificationsReceived, 1)
                        .And(domain.TheNotificationRecievedAtIndex_ShouldHaveResult_, 1, true)
                    .ExecuteWithReport();
            }
        }

        [Test]
        public void SqlQueryWithInterpretZeroRowsAsFailure()
        {
            using (var domain = new SqlQueryCheckDomain(new SqlQueryCheckConfig
                                               {
                                                   InterpretZeroRowsAsAFailure = true,
                                                   ConnectionString = "WolfpackR",
                                                   Query = "SELECT * FROM master.sys.databases WHERE 1=2"
                                               }))
            {
                Feature.WithScenario("returns no rows")
                    .Given(domain.TheAgentIsStarted)
                    .When(domain.TheHealthCheckIsInvoked)
                    .Then(domain.ThereShouldBe_HealthCheckNotificationsReceived, 1)
                        .And(domain.TheNotificationRecievedAtIndex_ShouldHaveResult_, 1, false)
                    .ExecuteWithReport();
            }
        }
    }
}