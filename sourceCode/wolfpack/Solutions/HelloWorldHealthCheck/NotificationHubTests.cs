using System;
using NUnit.Framework;
using Wolfpack.Core;
using Wolfpack.Core.Filters.Notification;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;

namespace HelloWorldHealthCheck
{
    [TestFixture]
    public class NotificationHubTests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            Messenger.Initialise(new MagnumMessenger());
            Messenger.InterceptBefore<HealthCheckResult>(msg =>
                                                           {
                                                               var bob = msg;
                                                           });
            Messenger.InterceptBefore<NotificationRequest>(msg =>
                                                           {
                                                               var bob = msg;
                                                           });

            Container.RegisterInstance(new FailureOnlyNotificationFilter());
        }

        [Test]
        public void Test()
        {           
            var hub = new NotificationHub();
            hub.Initialise(new AgentInfo());

            Messenger.Publish(NotificationRequest.For("FailureOnly",
                                                      HealthCheckData.For(new PluginDescriptor
                                                                              {
                                                                                  Name = "Test"
                                                                              }, "Test").Failed()));
        }
    }
}