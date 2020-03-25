using Castle.DynamicProxy;
using Wolfpack.Core.Containers;
using Wolfpack.Core.Interfaces.Castle;
using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.Interfaces.Magnum;

namespace Wolfpack.Core.Filters
{
    public abstract class ResultFilterBase : InterceptorBase, IPublisherFilter
    {
        public string Publisher { get; set; }
        public string Check { get; set; }

        protected ResultFilterBase()
            : base("Consume")
        {
        }

        protected abstract bool ShouldPublish(INotificationEventPublisher publisher, 
            NotificationEvent message);

        protected override void HandleIntercept(IInvocation invocation)
        {
            var message = invocation.GetArgumentValue(0) as NotificationEvent;
            var publisher = invocation.InvocationTarget as INotificationEventPublisher;

            if (Applies(publisher, message))
            {
                // run the filter...
                if (ShouldPublish(publisher, message))
                    invocation.Proceed();
            }
            else
            {
                invocation.Proceed();
            }
        }

        protected bool Applies(INotificationEventPublisher publisher, NotificationEvent message)
        {
            if ((message == null) || (publisher == null))
                return false;

            var publisherMatch = ((System.String.Compare(publisher.FriendlyId, Publisher, System.StringComparison.OrdinalIgnoreCase) == 0) ||
                                  (System.String.Compare(Publisher, "*", System.StringComparison.OrdinalIgnoreCase) == 0));
            var checkMatch = ((System.String.Compare(message.CheckId, Check, System.StringComparison.OrdinalIgnoreCase) == 0) ||
                    (System.String.Compare(Check, "*", System.StringComparison.OrdinalIgnoreCase) == 0));
            return publisherMatch && checkMatch;
        }        
    }
}