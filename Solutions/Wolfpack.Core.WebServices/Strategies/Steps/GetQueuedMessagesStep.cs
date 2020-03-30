using System.Linq;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.Repositories.Queries;
using Wolfpack.Core.WebServices.Interfaces.Entities;

namespace Wolfpack.Core.WebServices.Strategies.Steps
{
    /// <summary>
    /// This step will collect all unsent messages and add them to the queue
    /// </summary>
    public class GetQueuedMessagesStep : StepBase<WebServicePublisherContext>
    {
        private readonly INotificationRepository _repository;

        public GetQueuedMessagesStep(INotificationRepository repository)
        {
            _repository = repository;
        }

        public override ContinuationOptions Execute(WebServicePublisherContext context)
        {
            var messages = _repository.Filter(new NotificationByStateQuery(MessageStateTypes.Queued))
                .OrderBy(msg => msg.GeneratedOnUtc)
                .ToList();

            messages.ForEach(msg => context.OutboundQueue.Enqueue(msg));
            return ContinuationOptions.Continue;          
        }
    }
}