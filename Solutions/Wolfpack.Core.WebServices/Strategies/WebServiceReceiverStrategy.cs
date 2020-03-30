using System;
using System.Linq;
using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.Pipeline;
using Wolfpack.Core.WebServices.Interfaces;
using Wolfpack.Core.WebServices.Interfaces.Entities;
using Wolfpack.Core.WebServices.Interfaces.Exceptions;
using Wolfpack.Core.WebServices.Strategies.Steps;

namespace Wolfpack.Core.WebServices.Strategies
{
    public class WebServiceReceiverStrategy : WebServiceStrategyBase<WebServiceReceiverContext>,
        IWebServiceReceiverStrategy
    {
        public void Execute(NotificationEvent notification)
        {
            var pipeline = new DefaultPipeline<WebServiceReceiverContext>(
                Load<MessageStalenessCheckStep>(),
                Load<CheckForDuplicateStep>(),
                Load<RepublishMessageStep>());

            var result = pipeline.Execute(new WebServiceReceiverContext
                                              {
                                                  Notification = notification
                                              });

            if (result.Success)
                return;

            var ex = result.StepResults.FirstOrDefault(sr => sr.Failure != null);
            if (ex != null)
            {
                // ignore staleness exceptions
                if (ex.Failure.Error is StaleMessageException)
                    return;

                throw new ApplicationException("Step failure", ex.Failure.Error);
            }

            throw new ApplicationException("General Pipeline failure");
        }
    }
}