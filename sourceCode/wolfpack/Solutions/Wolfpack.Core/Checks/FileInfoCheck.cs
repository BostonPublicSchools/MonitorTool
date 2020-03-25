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
    public class FileInfoCheckConfig : PluginConfigBase, ISupportNotificationMode
    {
        public string FileLocation { get; set; }
        public string NotificationMode { get; set; }
    }

    public class FileInfoConfigurationAdvertiser : HealthCheckDiscoveryBase<FileInfoCheckConfig, FileInfoCheck>
    {
        protected override FileInfoCheckConfig GetConfiguration()
        {
            return new FileInfoCheckConfig
                       {
                           Enabled = true,
                           FriendlyId = "CHANGEME!",
                           NotificationMode = StateChangeNotificationFilter.FilterName,
                           FileLocation = "Fully qualified filename"
                       };
        }

        protected override void Configure(ConfigurationEntry entry)
        {
            entry.Name = "FileInfoCheck";
            entry.Description = "This check monitors a specific filename.";
            entry.Tags.AddIfMissing("HealthCheck", "FileSystem");
        }
    }

    public class FileInfoCheck : HealthCheckBase<FileInfoCheckConfig>
    {
        protected readonly SmartLocation FileLocation;

        /// <summary>
        /// default ctor
        /// </summary>
        public FileInfoCheck(FileInfoCheckConfig config) : base(config)
        {
            FileLocation = new SmartLocation(config.FileLocation);
        }

        public override void Execute()
        {
            var fi = new FileInfo(FileLocation.Location);
            var data = HealthCheckData.For(Identity, 
                "Information about file '{0}'...", FileLocation.Location)
                .ResultIs(fi.Exists);

            if (data.Result.GetValueOrDefault(false))
            {
                data.ResultCount = fi.Length;
                data.Properties = new Properties
                                      {
                                            {"CreationTimeUtc", fi.CreationTimeUtc.ToString(CultureInfo.InvariantCulture)},
                                            {"LastAccessTimeUtc", fi.LastAccessTimeUtc.ToString(CultureInfo.InvariantCulture)},
                                            {"LastWriteTimeUtc", fi.LastWriteTimeUtc.ToString(CultureInfo.InvariantCulture)},
                                            {"Length", fi.Length.ToString(CultureInfo.InvariantCulture)},
                                            {"Attributes", fi.Attributes.ToString()}
                                        };
            }

            Messenger.Publish(NotificationRequestBuilder.For(_config.NotificationMode, data).Build());
        }

        protected override PluginDescriptor BuildIdentity()
        {
            return new PluginDescriptor
            {
                Description = string.Format("Reports information about file {0}", FileLocation.Location),
                TypeId = new Guid("E0DDCF22-25B6-4d05-B2C4-D1EBFBEBB681"),
                Name = _config.FriendlyId
            };
        }
    }
}