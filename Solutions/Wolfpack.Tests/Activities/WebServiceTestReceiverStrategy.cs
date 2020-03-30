using System;
using System.Collections.Generic;
using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.WebServices.Interfaces;

namespace Wolfpack.Tests.Activities
{
    public class WebServiceTestReceiverStrategy : IWebServiceReceiverStrategy
    {
        public Dictionary<Guid, NotificationEvent> NotificationsReceived { get; private set; }

        public WebServiceTestReceiverStrategy()
        {
            NotificationsReceived = new Dictionary<Guid, NotificationEvent>();
        }
        public void Execute(NotificationEvent notification)
        {
            NotificationsReceived.Add(notification.Id, notification);
        }
    }
}