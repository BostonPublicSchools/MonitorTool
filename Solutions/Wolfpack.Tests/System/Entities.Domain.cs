using System;
using System.Collections.Generic;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Wolfpack.Core;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.Notification;
using Wolfpack.Core.Testing.Bdd;
using System.Linq;

namespace Wolfpack.Tests.System
{
    public class EntitiesDomain : BddTestDomain
    {
        private readonly Fixture _autodata;
        private string _xmlFilePath;

        private readonly List<INotificationRequestFilter> _filters;
        private NotificationEventHealthCheck _resultEntity;

        private NotificationRequest _request;
        private NotificationEvent _event;

        public EntitiesDomain()
        {
            _autodata = new Fixture();
            _autodata.Customize<NotificationRequest>(x => x.Without(nr => nr.Notification));
            _filters = new List<INotificationRequestFilter>();
        }

        public override void Dispose()
        {                        
        }

        public void TheHealthCheckResultDataFilenameIsBuiltForVersion_(string version)
        {
            _xmlFilePath = string.Format(@"TestData\{0}\HealthCheckResult.xml", version);
        }

        public void TheHealthCheckResultXmlIsDeserialised()
        {
            SafeExecute(() => _resultEntity = Serialiser.FromXmlInFile<NotificationEventHealthCheck>(_xmlFilePath));
        }

        public void TheResultShouldNotBeNull()
        {
            Assert.That(_resultEntity, Is.Not.Null);
        }

        public void TheNotificationRequestIsFullyPopulated()
        {
            _request = _autodata.Build<NotificationRequest>()
                .Do(nr => nr.Notification = _autodata.Create<NotificationEvent>())
                .Create();
        }

        public void TheNotificationRequestIsConvertedToAnEvent()
        {
            var hub = new DefaultNotificationHub(new AgentConfiguration(), _filters);
            _event = hub.ConvertRequestToEvent(_request);
        }

        public void AllTheNotificationEventPropertiesMatchTheRequestNotification()
        {
            var sourceProperties = typeof(INotificationEventCore).GetProperties();
            var targetProperties = _event.GetType().GetProperties();

            foreach (var rpi in sourceProperties)
            {
                var targetProp = targetProperties.SingleOrDefault(epi => epi.Name.Equals(rpi.Name));
                if (targetProp == null)
                    throw new InvalidOperationException(
                        string.Format("NotificationRequest.Notification property '{0}' does not exist on target NotificiationEvent",
                                      rpi.Name));

                Assert.That(rpi.GetValue(_request.Notification, null), Is.EqualTo(targetProp.GetValue(_event, null)));
            }
        }
    }
}