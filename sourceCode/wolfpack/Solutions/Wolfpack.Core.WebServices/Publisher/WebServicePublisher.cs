using System;
using Magnum.Pipeline;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.Schedulers;
using Wolfpack.Core.WebServices.Interfaces;
using Wolfpack.Core.WebServices.Interfaces.Entities;

namespace Wolfpack.Core.WebServices.Publisher
{
    public class WebServicePublisher : IntervalSchedulerBase, IActivityPlugin, IConsumer<NotificationEvent>
    {
        private readonly WebServicePublisherConfig _config;
        private readonly IWebServicePublisherStrategy _strategy;
        private readonly INotificationRepository _notificationRepository;

        public WebServicePublisher(WebServicePublisherConfig config, 
            INotificationRepository notificationRepository,
            IWebServicePublisherStrategy strategy) 
            : base(new IntervalSchedulerConfig
                       {
                           IntervalInSeconds = config.SendIntervalInSeconds 
                           ?? WebServicePublisherConfig.DefaultSendInterval
                       })
        {
            _config = config;
            _notificationRepository = notificationRepository;
            _strategy = strategy;
        }

        public bool Enabled
        {
            get { return _config.Enabled; }
            set { _config.Enabled = value; }
        }

        public override void Initialise()
        {
            Messenger.Subscribe(this);
        }

        public void Consume(NotificationEvent message)
        {
            // store and forward...
            message.State = MessageStateTypes.Queued;
            _notificationRepository.Add(message);
        }

        public override PluginDescriptor Identity
        {
            get
            {
                return new PluginDescriptor
                {
                    Name = "Wolfpack WebService Publisher Activity",
                    TypeId = new Guid("89FC3EC5-A9D5-496A-958F-E9D628C3E133"),
                    Description = "Sends notifications between Agents"
                };
            }
        }

        protected override void Execute()
        {
            _strategy.Execute();
        }
    }
}