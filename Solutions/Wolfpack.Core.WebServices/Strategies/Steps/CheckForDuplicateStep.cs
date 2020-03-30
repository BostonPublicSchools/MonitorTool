using System;
using System.Collections.Generic;
using System.Linq;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.WebServices.Interfaces.Entities;

namespace Wolfpack.Core.WebServices.Strategies.Steps
{
    public class CheckForDuplicateStep : StepBase<WebServiceReceiverContext>
    {
        private readonly IEnumerable<INotificationRepository> _repositories;

        public CheckForDuplicateStep(IEnumerable<INotificationRepository> repositories)
        {
            _repositories = repositories;
        }

        public override ContinuationOptions Execute(WebServiceReceiverContext context)
        {
            // throw duplicatemessageexception if notification already exists in ALL repositories
            if (_repositories.All(r => ExistsIn(r, context.Notification.Id)))
            {
                Logger.Warning("Message '{0}' already exists, discarding it!", context.Notification.Id);
                // stop execution of the pipeline
                return ContinuationOptions.Stop;   
            }

            return ContinuationOptions.Continue;   
        }

        private bool ExistsIn(INotificationRepository repository, Guid notificationId)
        {
            NotificationEvent notification;
            if (repository.GetById(notificationId, out notification))
            {
                Logger.Info("Message '{0}' has already been received (on {1}), discarding it! {2}",
                    notificationId,
                    notification.ReceivedOnUtc,
                    repository.GetType().Name);
                return true;
            }
            return false;
        }
    }
}