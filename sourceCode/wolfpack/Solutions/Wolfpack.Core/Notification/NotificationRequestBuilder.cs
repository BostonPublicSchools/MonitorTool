using System;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Notification
{
    public class NotificationRequestBuilder
    {
        private NotificationRequest _request;

        private NotificationRequestBuilder()
        {
            _request = new NotificationRequest();
        }

        public static NotificationRequestBuilder AlwaysPublish(HealthCheckData message)
        {
            return For(String.Empty, message);
        }

        public static NotificationRequestBuilder AlwaysPublish(INotificationEventCore message)
        {
            return For(String.Empty, message);
        }

        public static NotificationRequestBuilder For(string notificationMode, 
            HealthCheckData message, 
            Action<NotificationRequest> configurer = null)
        {
            var alert = new NotificationEventHealthCheck
            {
                CheckId = message.Identity.Name,
                CriticalFailure = message.CriticalFailure,
                CriticalFailureDetails = message.CriticalFailureDetails,
                Duration = message.Duration,
                GeneratedOnUtc = message.GeneratedOnUtc,
                Message = message.Info,
                NextCheckExpected = message.NextCheckExpected,
                Properties = message.Properties,
                Result = message.Result,
                ResultCount = message.ResultCount,
                DisplayUnit = message.DisplayUnit,
                Tags = message.Tags
            };

            var builder = For(notificationMode, alert);
            
            if (configurer != null)
                configurer(builder._request);
            return builder;
        }

        public static NotificationRequestBuilder For(string notificationMode, INotificationEventCore message)
        {
            return new NotificationRequestBuilder
            {
                _request = new NotificationRequest
                {
                    Notification = message,
                    Mode = notificationMode
                }
            };
        }

        public NotificationRequestBuilder AssociateArtifact(ArtifactDescriptor artifactDescriptor)
        {
            _request.AssociateArtifact(artifactDescriptor);
            return this;
        }

        public NotificationRequest Build()
        {
            Validate();
            return _request;
        }

        private void Validate()
        {
            // TODO: validation of the request
        }
    }
}