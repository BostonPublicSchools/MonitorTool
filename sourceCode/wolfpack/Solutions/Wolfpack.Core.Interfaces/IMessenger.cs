using System;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Interfaces
{
    public interface IMessenger
    {
        IMessenger Publish<T>(T message) where T: class;
        IMessenger Publish(NotificationRequest message);
        IMessenger Publish(NotificationEvent message);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Target type to consume the message</typeparam>
        /// <param name="consumer"></param>
        /// <returns></returns>
        IMessenger Subscribe<T>(T consumer) where T : class;

        IMessenger InterceptBefore<T>(Action<T> action) where T : class;
    }
}