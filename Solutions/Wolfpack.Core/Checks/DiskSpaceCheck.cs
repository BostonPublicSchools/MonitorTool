using System;
using System.Runtime.InteropServices;
using Wolfpack.Core.Configuration;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.Notification;
using Wolfpack.Core.Notification.Filters.Request;

namespace Wolfpack.Core.Checks
{
    public class DiskSpaceCheckConfig : PluginConfigBase, ISupportNotificationMode, ISupportNotificationThreshold
    {
        public string Path { get; set; }
        public string NotificationMode { get; set; }
        public double? NotificationThreshold { get; set; }
    }

    public class DiskSpaceConfigurationAdvertiser : HealthCheckDiscoveryBase<DiskSpaceCheckConfig, DiskSpaceCheck>
    {
        protected override DiskSpaceCheckConfig GetConfiguration()
        {
            return new DiskSpaceCheckConfig
            {
                Enabled = true,
                FriendlyId = "CHANGEME!",
                NotificationMode = StateChangeNotificationFilter.FilterName,
                NotificationThreshold = 80,
                Path = @"C:\"
            };
        }

        protected override void Configure(ConfigurationEntry entry)
        {
            entry.Name = "DiskSpaceUtilisation";
            entry.Description = "This check monitors the free disk space. Typically this check is configured to start producing notifications once a threshold % is breached.";
            entry.Tags.AddIfMissing("Disk", "Threshold");
        }
    }


    public class DiskSpaceCheck : ThresholdCheckBase<DiskSpaceCheckConfig>
    {
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetDiskFreeSpaceEx(string lpDirectoryName,
        out ulong lpFreeBytesAvailable,
        out ulong lpTotalNumberOfBytes,
        out ulong lpTotalNumberOfFreeBytes);

        public DiskSpaceCheck(DiskSpaceCheckConfig config)
            : base(config)
        {
        }

        public override void Initialise()
        {
            // http://msdn.microsoft.com/en-us/library/aa364937%28VS.85%29.aspx
            // UNC paths must be \ terminated            
            if (_config.Path.StartsWith(@"\\"))
                _config.Path = _config.Path.TrimEnd(@"\") + @"\";
        }

        public override void Execute()
        {
            ulong freeBytesAvailable;
            ulong totalNumberOfBytes;
            ulong totalNumberOfFreeBytes;

            var success = GetDiskFreeSpaceEx(_config.Path, out freeBytesAvailable, out totalNumberOfBytes, out totalNumberOfFreeBytes);
            if (!success)
                throw new System.ComponentModel.Win32Exception(string.Format("GetDiskFreeSpaceEx({0}) failed", _config.Path));

            var usedpercentage = 100 - (freeBytesAvailable*100)/totalNumberOfBytes;

            Logger.Debug(FormatMessage(usedpercentage));

            Publish(NotificationRequestBuilder.For(_config.NotificationMode, HealthCheckData.For(Identity,
                FormatMessage(usedpercentage))
                .Succeeded()
                .ResultCountIs(usedpercentage)
                .DisplayUnitIs("%")
                .AddTag(_config.Path))
                .Build());
        }

        private string FormatMessage(ulong usedpercentage)
        {
            return string.Format("Disk space used on '{0}' is {1}%{2}", _config.Path, usedpercentage,
                                 _config.NotificationThreshold.HasValue
                                     ? string.Format(" (Threshold={0}%)", _config.NotificationThreshold.Value)
                                     : string.Empty);
        }

        protected override PluginDescriptor BuildIdentity()
        {
            return new PluginDescriptor
            {
                Description = "Reports the used disk space as %",
                TypeId = new Guid("F99F603F-B010-450D-8860-248340003E58"),
                Name = _config.FriendlyId
            };
        }
    }
}