using System;
using System.Collections.Generic;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Notification.Filters.Request
{
    public class StateChangeNotificationFilter : NotificationRequestFilterBase
    {
        public const string FilterName = "StateChange";

        protected readonly Dictionary<string, AlertHistory> History;

        public StateChangeNotificationFilter()
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
                    alertHistory = new AlertHistory
                    {
                        Result = request.Notification.Result,
                        Received = DateTime.UtcNow
                    };

                    var result = request.Notification.Result.GetValueOrDefault(true);

                    if (!result)
                        alertHistory.FailuresSinceLastSuccess++;

                    HandleFirstAlert(request, alertHistory);
                    // only publish if initial failure result
                    return !result;
                }

                // has state change from last?
                alertHistory = History[key];

                if (!HasStateChanged(request, alertHistory))
                {
                    Logger.Debug("State for Check '{0}' is unchanged, not publishing", request.CheckId);
                    return false;
                }

                HandleStateChange(request, alertHistory);
                return true;
            }
        }

        protected virtual void HandleFirstAlert(NotificationRequest request, AlertHistory alertHistory)
        {
            History.Add(GetKey(request), alertHistory);
        }

        protected virtual bool HasStateChanged(NotificationRequest request, AlertHistory alertHistory)
        {
            return alertHistory.Result != request.Notification.Result;
        }

        protected virtual void HandleStateChange(NotificationRequest request, AlertHistory alertHistory)
        {
            alertHistory.Result = request.Notification.Result;
            alertHistory.Received = DateTime.UtcNow;
        }
    }
}
