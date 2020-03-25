using System;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Checks
{
    /// <summary>
    /// This provides a base class for any HealthCheck that utilises a threshold
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ThresholdCheckBase<T> : HealthCheckBase<T>
        where T : ISupportNotificationMode, ISupportNotificationThreshold
    {
        private readonly Func<double, double, bool> _comparer;

        protected ThresholdCheckBase(T config) : base(config)
        {
            _comparer = AboveThreshold;
        }

        protected ThresholdCheckBase(T config, bool thresholdBreachedIfValueLower) : this(config)
        {
            if (thresholdBreachedIfValueLower)
                _comparer = BelowThreshold;
        }

        protected override void Publish(NotificationRequest request)
        {
            if (IsThresholdBreached(request.Notification.ResultCount))
                AdjustMessageForBreach(request.Notification);

            base.Publish(request);
        }

        /// <summary>
        /// By default a breached threshold is a failure, override this method to 
        /// provide custom logic to set message properties in the event of a breach
        /// </summary>
        /// <param name="message"></param>
        protected virtual void AdjustMessageForBreach(INotificationEventCore message)
        {
            message.Result = false;
        }

        protected virtual bool IsThresholdBreached(double? resultCount)
        {
            if (!_config.NotificationThreshold.HasValue)
                return true;
            if (!resultCount.HasValue)
                return true;
            return _comparer(resultCount.Value, _config.NotificationThreshold.Value);
        }

        protected virtual bool AboveThreshold(double resultCount, double threshold)
        {
            return (resultCount > threshold);
        }

        protected virtual bool BelowThreshold(double resultCount, double threshold)
        {
            return (resultCount < threshold);
        }
    }
}