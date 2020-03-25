using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wolfpack.Core.Artifacts;
using Wolfpack.Core.Artifacts.Formats;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.Notification.Filters.Request;
using Wolfpack.Core.Testing.Bdd;
using Wolfpack.Core.Testing.Drivers;
using FluentAssertions;

namespace Wolfpack.Core.Testing.Domains
{
    /// <summary>
    /// This provides a base class that exposes the core wolfpack system
    /// </summary>
    public abstract class SystemDomainBase : BddTestDomain
    {
        public const string ArtifactBaseFolder = "_artifacts";

        protected readonly IList<IArtifactFormatter> _formatters;

        protected AutomationProfile Agent { get { return _automatedAgent; } }

        private readonly AutomationProfile _automatedAgent;

        private IArtifactManager _artifactManager;
        private IConfigurationManager _configurationManager;

        protected SystemDomainBase()
        {
            _automatedAgent = AutomationProfile.Configure();
            _formatters = new List<IArtifactFormatter>();
        }

        public override void Dispose()
        {
            // override this to perform something at the end of the test
            TheAgentIsStopped();
        }

        public void ThereShouldBe_SessionMessagesPublished(int expected)
        {
            Agent.NotificationEvents.Count(ne => ne.EventType == NotificationEventAgentStart.EventTypeName).Should().Be(expected);
        }

        public void ThereShouldBe_HealthCheckNotificationsReceived(int expected)
        {
            Agent.NotificationEvents.Count(ne => ne.EventType == NotificationEventHealthCheck.EventTypeName).Should().Be(expected);
        }

        public void ThereShouldBe_NotificationRequestMessagesOfType_Published(int expected, string type)
        {
            Agent.NotificationRequests.Count(m => m.Notification.EventType.Equals(type, StringComparison.OrdinalIgnoreCase))
                .Should().Be(expected);
        }

        public void TheNotificationRequestAtIndex_ShouldHaveResult_(int index, bool expected)
        {
            Agent.NotificationRequests[index].Notification.Result.Should().Be(expected);
        }

        public void TheNotificationRecievedAtIndex_ShouldHaveResult_(int index, bool expected)
        {
            Agent.NotificationEvents[index].Result.Should().Be(expected);
        }

        public void TheNotificationReceivedAtIndex_ShouldHaveResultCount_(int index, double expected)
        {
            Agent.NotificationEvents[index].ResultCount.Value.Should().BeInRange(expected, expected);
        }

        public void TheDefaultNotificationFiltersAreLoaded()
        {
            Agent.Run(new FailureOnlyNotificationFilter(),
                      new StateChangeNagFailNotificationFilter(),
                      new StateChangeNotificationFilter(),
                      new SuccessOnlyNotificationFilter());
        }

        public void TheAgentIsStarted()
        {
            if (_artifactManager == null)
                TheDefaultArtifactManagerIsLoaded();

            Agent.Start();
        }

        public void TheAgentIsStopped()
        {
            Agent.Stop();
        }

        public void TheCheckArtifactsArePurged(string checkName)
        {
            var path = Path.Combine(ArtifactBaseFolder, checkName);

            if (!Directory.Exists(path))
                return;

            foreach (var file in Directory.GetFiles(path))
            {
                File.Delete(file);
            }
        }

        public void ThereShouldBe_ArtifactFilesForCheck_(int expected, string checkName)
        {
            Directory.GetFiles(Path.Combine(ArtifactBaseFolder, checkName)).Count().Should().Be(expected);
        }

        public void TheDefaultConfigurationManagerIsLoaded()
        {
            Agent.Run(new AutomationConfigurationManager());
        }

        public void TheDefaultArtifactManagerIsLoaded()
        {
            _formatters.Add(new TabSeparatedFormatter());
            _formatters.Add(new JsonFormatter());
            _artifactManager = new FileSystemArtifactManager(ArtifactBaseFolder, _formatters);
            ArtifactManager.Initialise(_artifactManager);
        }
    }
}