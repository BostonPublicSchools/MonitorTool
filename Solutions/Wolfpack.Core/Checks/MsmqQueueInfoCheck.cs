using System;
using System.Globalization;
using System.Messaging;
using Wolfpack.Core.Configuration;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.Notification;
using Wolfpack.Core.Notification.Filters.Request;

namespace Wolfpack.Core.Checks
{
    public class MsmqQueueInfoCheckConfig : PluginConfigBase, ISupportNotificationMode, ISupportNotificationThreshold
    {
        public string QueueName { get; set; }
        public string NotificationMode { get; set; }
        public double? NotificationThreshold { get; set; }
    }

    public class MsmqQueueInfoConfigurationAdvertiser : HealthCheckDiscoveryBase<MsmqQueueInfoCheckConfig, MsmqQueueInfoCheck>
    {
        protected override MsmqQueueInfoCheckConfig GetConfiguration()
        {
            return new MsmqQueueInfoCheckConfig
                       {
                           Enabled = true,
                           FriendlyId = "CHANGEME!",
                           NotificationMode = StateChangeNotificationFilter.FilterName,
                           NotificationThreshold = 0,
                           QueueName = "CHANGEME!"
                       };
        }

        protected override void Configure(ConfigurationEntry entry)
        {
            entry.Name = "MsmqQueueInfoCheck";
            entry.Description = "This check will monitor a MSMQ Queue for breaches of too many items. If there are more items than the threshold set it will generate failure notifications.";
            entry.Tags.AddIfMissing("MSMQ", "Threshold");
        }
    }

    public class MsmqQueueInfoCheck : ThresholdCheckBase<MsmqQueueInfoCheckConfig>
    {       
        /// <summary>
        /// default ctor
        /// </summary>
        /// <param name="config"></param>
        public MsmqQueueInfoCheck(MsmqQueueInfoCheckConfig config) : base(config)
        {
        }

        protected override PluginDescriptor BuildIdentity()
        {
            return new PluginDescriptor
            {
                Description = string.Format("Gathers information about the queue '{0}'", _config.QueueName),
                TypeId = new Guid("7C6F11A6-7265-4aee-AB23-BA549EACB592"),
                Name = _config.FriendlyId
            };
        }

        public override void  Execute()
        {
            Publish(DoTest());
        }

        protected virtual NotificationRequest DoTest()
        {
            string info;
            var count = 0;
            DateTime? oldestMessageDated = null;

            var exists = IsDeadLetterRelatedQueue() || MessageQueue.Exists(_config.QueueName);            
            
            if (exists)
            {
                var queue = new MessageQueue(_config.QueueName)
                {
                    MessageReadPropertyFilter = new MessagePropertyFilter
                    {
                        ArrivedTime = true
                    }
                };

                var me = queue.GetMessageEnumerator2();
                var timeout = new TimeSpan(0, 0, 0);

                while (me.MoveNext(timeout))
                {
                    var msg = me.Current;

                    if (msg == null)
                        continue;

                    if (!oldestMessageDated.HasValue)
                        oldestMessageDated = msg.ArrivedTime;
                    else if (msg.ArrivedTime < oldestMessageDated)
                        oldestMessageDated = msg.ArrivedTime;

                    count++;
                }

                info = string.Format("Queue {0} has {1} messages", _config.QueueName, count);
            }
            else
            {
                info = string.Format("Queue {0} does not exist!", _config.QueueName);
            }

            var props = new Properties
                            {
                                {"Queue", _config.QueueName},
                                {"Count", count.ToString(CultureInfo.InvariantCulture)}
                            };

            if (oldestMessageDated.HasValue)
                props.Add("Oldest", oldestMessageDated.Value.ToString(CultureInfo.InvariantCulture));

            var data = new HealthCheckData
            {
                Identity = Identity,
                Info = info,
                Result = exists,
                Properties = props
            };

            if (exists)
                data.ResultCount = count;

            return NotificationRequestBuilder.For(_config.NotificationMode, data).Build();
        }

        private bool IsDeadLetterRelatedQueue()
        {
            return _config.QueueName.ToLower().Contains(";deadletter")
                   || _config.QueueName.ToLower().Contains(";deadxact");
        }
    }
}