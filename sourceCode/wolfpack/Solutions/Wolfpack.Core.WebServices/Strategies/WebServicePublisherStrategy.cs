using System;
using System.Linq;
using Wolfpack.Core.Pipeline;
using Wolfpack.Core.WebServices.Interfaces;
using Wolfpack.Core.WebServices.Interfaces.Entities;
using Wolfpack.Core.WebServices.Strategies.Steps;

namespace Wolfpack.Core.WebServices.Strategies
{
    public class WebServicePublisherStrategy : WebServiceStrategyBase<WebServicePublisherContext>,
        IWebServicePublisherStrategy
    {
        public void Execute()
        {
            var pipeline = new DefaultPipeline<WebServicePublisherContext>(
                Load<GetQueuedMessagesStep>(),
                Load<SendMessagesStep>(),
                Load<CleanupMessagesStep>());

            var result = pipeline.Execute(new WebServicePublisherContext());

            if (result.Success)
                return;

            var ex = result.StepResults.FirstOrDefault(sr => sr.Failure != null);
            if (ex != null)
            {
                throw new ApplicationException("Step failure", ex.Failure.Error);
            }

            throw new ApplicationException("Pipeline failure");
        }
    }
}