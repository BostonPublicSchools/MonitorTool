using System.Linq;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.WebServices.Interfaces.Entities;

namespace Wolfpack.Core.WebServices.Strategies.Steps
{
    /// <summary>
    /// This step will collect all unsent messages and add them to the queue
    /// </summary>
    public class CleanupMessagesStep : StepBase<WebServicePublisherContext>
    {
        private readonly INotificationRepository _repository;

        public CleanupMessagesStep(INotificationRepository repository)
        {
            _repository = repository;
        }

        public override bool ShouldExecute(WebServicePublisherContext context)
        {
            return context.DeliveredQueue.Any();
        }

        public override ContinuationOptions Execute(WebServicePublisherContext context)
        {
            Logger.Info("{0} notifications delivered...", context.DeliveredQueue.Count);

            while (context.DeliveredQueue.Any())
            {
                var message = context.DeliveredQueue.Dequeue();
                _repository.Delete(message.Id);
            }
            return ContinuationOptions.Continue;          
        }
    }
}