using System.Collections.Generic;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.WebServices.Interfaces.Entities
{
    public class WebServicePublisherContext
    {
        public Queue<NotificationEvent> OutboundQueue { get; set; }
        public Queue<NotificationEvent> DeliveredQueue { get; set; }

        public WebServicePublisherContext()
        {
            OutboundQueue = new Queue<NotificationEvent>();
            DeliveredQueue = new Queue<NotificationEvent>();
        }
    }
}