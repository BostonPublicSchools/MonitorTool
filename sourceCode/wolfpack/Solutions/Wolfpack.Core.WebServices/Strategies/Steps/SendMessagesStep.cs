using System;
using System.Linq;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.WebServices.Interfaces;
using Wolfpack.Core.WebServices.Interfaces.Entities;

namespace Wolfpack.Core.WebServices.Strategies.Steps
{
    public class SendMessagesStep : StepBase<WebServicePublisherContext>
    {
        private readonly IWolfpackWebServicesClient _client;

        public SendMessagesStep(IWolfpackWebServicesClient client)
        {
            _client = client;
        }

        public override bool ShouldExecute(WebServicePublisherContext context)
        {
            return context.OutboundQueue.Any();
        }

        public override ContinuationOptions Execute(WebServicePublisherContext context)
        {
            Logger.Info("{0} notifications queued...", context.OutboundQueue.Count);

            while (context.OutboundQueue.Any())
            {
                var message = context.OutboundQueue.Dequeue();

                try
                {
                    _client.Deliver(message);
                    context.DeliveredQueue.Enqueue(message);
                }
                catch (Exception ex)
                {
                    var msg = Logger.Event.During("WebServicePublisher, SendMessagesStep")
                        .Description("Critical failure sending notification ({0}) {1}, reason: {2}", message.EventType,
                                     message.Id, ex.Message)
                        .Encountered(ex)
                        .StateIsFailure();
                    Logger.Error(msg);
                }
            }

            return ContinuationOptions.Continue;
        }
    }
}
