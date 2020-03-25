using System;
using Wolfpack.Core;
using Wolfpack.Core.Checks;
using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.Notification;

namespace HelloWorldHealthCheck
{
    public class HelloWorldCheck : HealthCheckBase<HelloWorldCheckConfig>
    {
        public HelloWorldCheck(HelloWorldCheckConfig config) 
            : base(config)
        {
        }

        public override void Execute()
        {
            Messenger.Publish(NotificationRequestBuilder.AlwaysPublish(new HealthCheckData
                        {
                            Identity = Identity,
                            Result = true,
                            Info = string.Format("MyCustomSetting:={0}", _config.MyCustomSetting)
                        }).Build());
        }

        protected override PluginDescriptor BuildIdentity()
        {
            return new PluginDescriptor
            {
                Description = "My first custom HealthCheck!",
                Name = _config.FriendlyId,
                // The TypeId is important - it needs to be different for
                // each health check as this allows us to positively identify
                // every health check. Use the VS Tools/Create GUID tool to
                // generate a new one.
                TypeId = new Guid("218087BB-3605-4fa5-9157-0C133674F51F")
            };
        }
    }
}
