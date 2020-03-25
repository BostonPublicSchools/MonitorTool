using System;
using System.Collections.Generic;
using Wolfpack.Core.Interfaces.Entities;
using Omu.ValueInjecter;

namespace Wolfpack.Core.Notification.Filters.Request
{
    public class ResultCountChangeNotificationFilter : NotificationRequestFilterBase
    {
        public const string FilterName = "ResultCountChange";

        protected readonly Dictionary<string, AlertHistory> History;

        public ResultCountChangeNotificationFilter()
        {
            History = new Dictionary<string, AlertHistory>();
        }

        public override string Mode { get { return FilterName; } }

        public override bool Execute(NotificationRequest request)
        {
            lock (History)
            {
                var key = GetKey(request);
                AlertHistory alertHistory;

                if (!History.ContainsKey(key))
                {
                    alertHistory = new AlertHistory { Received = DateTime.UtcNow };
                    HandleFirstAlert(request, alertHistory);
                    return true;
                }

                // has count change from last?
                alertHistory = History[key];

                if (!HasNotificationChanged(request, alertHistory))
                {
                    Logger.Debug("ResultCount for Check '{0}' is unchanged, not publishing", request.CheckId);
                    return false;
                }

                HandleStateChange(request, alertHistory);
                return true;
            }
        }

        protected virtual void HandleFirstAlert(NotificationRequest request, AlertHistory alertHistory)
        {
            alertHistory.InjectFrom<LoopValueInjection>(request.Notification);
            History.Add(GetKey(request), alertHistory);
        }

        protected virtual bool HasNotificationChanged(NotificationRequest request, AlertHistory alertHistory)
        {
            return !alertHistory.ResultCount.GetValueOrDefault(0).Equals(request.Notification.ResultCount.GetValueOrDefault(0));
        }

        protected virtual void HandleStateChange(NotificationRequest request, AlertHistory alertHistory)
        {
            alertHistory.InjectFrom<LoopValueInjection>(request.Notification);
            alertHistory.Received = DateTime.UtcNow;
        }
    }
}
