using System;
using Magnum.Pipeline;
using Magnum.Pipeline.Segments;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core
{
    /// <summary>
    /// This implementation uses the Magnum Event Aggregator/Bus for
    /// decoupled intercomponent communication
    /// </summary>
    public static class Messenger
    {
        private static IMessenger _instance;

        public static void Initialise()
        {
            if (!Container.IsRegistered<IMessenger>())
            {
                Container.RegisterAsSingleton<IMessenger>(typeof(MagnumMessenger));
            }

            Initialise(Container.Resolve<IMessenger>());
        }

        public static void Initialise(IMessenger instance)
        {
            _instance = instance; 
        }

        public static IMessenger Publish<T>(T message)
             where T : class
        {
            return _instance.Publish(message);
        }

        public static IMessenger Publish(NotificationRequest request)
        {
            return _instance.Publish(request);
        }

        public static IMessenger Publish(NotificationEvent message)
        {
            return _instance.Publish(message);           
        }

        public static IMessenger Subscribe<T>(T consumer) where T: class
        {
            return _instance.Subscribe(consumer);
        }

        public static IMessenger InterceptBefore<T>(Action<T> action) where T : class
        {
            return _instance.InterceptBefore(action);
        }
    }

    /// <summary>
    /// This implementation uses the Magnum Event Aggregator/Bus for
    /// decoupled intercomponent communication
    /// </summary>
    public class MagnumMessenger : IMessenger
    {
        private readonly InputSegment _messageBus;
        private readonly ISubscriptionScope _subscriptionScope;

        public MagnumMessenger()
        {
            _messageBus = PipeSegment.Input(PipeSegment.End());
            _subscriptionScope = _messageBus.NewSubscriptionScope();
        }

        public IMessenger Publish<T>(T message) where T : class
        {
            _messageBus.Send(message);
            return this;
        }

        IMessenger IMessenger.Publish(NotificationRequest message)
        {
            _messageBus.Send(message);
            return this;
        }

        IMessenger IMessenger.Publish(NotificationEvent message)
        {
            _messageBus.Send(message);
            return this;
        }

        public IMessenger Subscribe<T>(T consumer) where T : class
        {
            _subscriptionScope.Subscribe(consumer);
            return this;
        }

        public IMessenger InterceptBefore<T>(Action<T> action) where T : class
        {
            _subscriptionScope.Intercept<object>(
                config => config.BeforeEachMessage(
                    message =>
                    {
                        if (message.GetType() != typeof(T))
                            return;
                        action((T)message);
                    }));
            return this;
        }
    }
}