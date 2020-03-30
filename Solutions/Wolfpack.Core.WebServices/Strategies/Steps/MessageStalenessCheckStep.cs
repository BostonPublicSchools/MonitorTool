using System;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.WebServices.Interfaces.Entities;
using Wolfpack.Core.WebServices.Interfaces.Exceptions;

namespace Wolfpack.Core.WebServices.Strategies.Steps
{
    public class MessageStalenessCheckStep : StepBase<WebServiceReceiverContext>
    {
        private readonly MessageStalenessCheckConfig _config;

        public MessageStalenessCheckStep(MessageStalenessCheckConfig config)
        {
            _config = config;
        }

        public override ContinuationOptions Execute(WebServiceReceiverContext context)
        {
            if (DateTime.UtcNow.Subtract(context.Notification.GeneratedOnUtc).TotalMinutes > _config.MaxAgeInMinutes)
            {
                Logger.Warning("Message '{0}' is stale ({1} minutes old), discarding it!",
                    context.Notification.Id, DateTime.UtcNow.Subtract(context.Notification.GeneratedOnUtc).TotalMinutes);
                
                // stop execution of the pipeline
                return ContinuationOptions.Stop;
            }

            return ContinuationOptions.Continue;
        }
    }
}