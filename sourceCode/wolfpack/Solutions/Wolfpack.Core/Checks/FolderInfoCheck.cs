using System;
using System.Globalization;
using System.IO;
using Wolfpack.Core.Configuration;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.Notification;
using Wolfpack.Core.Notification.Filters.Request;

namespace Wolfpack.Core.Checks
{
    public class FolderInfoCheckConfig : PluginConfigBase, ISupportNotificationMode
    {
        public string FolderLocation { get; set; }
        public string NotificationMode { get; set; }
    }

    public class FolderInfoCheckConfigurationAdvertiser : HealthCheckDiscoveryBase<FolderInfoCheckConfig, FolderInfoCheck>
    {
        protected override FolderInfoCheckConfig GetConfiguration()
        {
            return new FolderInfoCheckConfig
                       {
                           Enabled = true,
                           FriendlyId = "CHANGEME!",
                           FolderLocation = "CHANGEME!",
                           NotificationMode = StateChangeNotificationFilter.FilterName
                       };
        }

        protected override void Configure(ConfigurationEntry entry)
        {
            entry.Name = "FolderInfoCheck";
            entry.Description = "This check monitors a folder. If it exists a success alert is created and metadata about the folder is returned; if it does not exist a failure alert is generated.";
            entry.Tags.AddIfMissing("Folder", "FileSystem");
        }
    }

    public class FolderInfoCheck : HealthCheckBase<FolderInfoCheckConfig>
    {
        protected readonly SmartLocation FolderLocation;

        /// <summary>
        /// default ctor
        /// </summary>
        public FolderInfoCheck(FolderInfoCheckConfig config) : base(config)
        {
            FolderLocation = new SmartLocation(config.FolderLocation);            
        }

        public override void Execute()
        {
            var fi = new DirectoryInfo(FolderLocation.Location);
            var data = HealthCheckData.For(Identity, "Information about folder '{0}'...", FolderLocation.Location)
                .ResultIs(fi.Exists);

            if (data.Result.GetValueOrDefault(false))
            {
                data.ResultCount = fi.GetFiles().LongLength;
                data.Properties = new Properties
                                      {
                                            {"CreationTimeUtc", fi.CreationTimeUtc.ToString(CultureInfo.InvariantCulture)},
                                            {"LastAccessTimeUtc", fi.LastAccessTimeUtc.ToString(CultureInfo.InvariantCulture)},
                                            {"LastWriteTimeUtc", fi.LastWriteTimeUtc.ToString(CultureInfo.InvariantCulture)},
                                            {"FileCount", fi.GetFiles().LongLength.ToString(CultureInfo.InvariantCulture)},
                                            {"FolderCount", fi.GetDirectories().LongLength.ToString(CultureInfo.InvariantCulture)},
                                            {"Attributes", fi.Attributes.ToString()}
                                        };
            }

            Messenger.Publish(NotificationRequestBuilder.For(_config.NotificationMode, data).Build());
        }

        protected override PluginDescriptor BuildIdentity()
        {
            return new PluginDescriptor
            {
                Description = string.Format("Reports information about folder {0}", FolderLocation.Location),
                TypeId = new Guid("5A3F5E7D-28B6-4ce9-A77A-66E83FEB888A"),
                Name = _config.FriendlyId
            };
        }
    }
}