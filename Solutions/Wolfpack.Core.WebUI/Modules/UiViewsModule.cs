using System;
using System.Collections.Generic;
using Nancy;
using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.WebServices;
using Wolfpack.Core.WebServices.Interfaces.Entities;
using Wolfpack.Core.WebUI.Interfaces.Entities;

namespace Wolfpack.Core.WebUI.Modules
{
    public class UiViewsModule : NancyModule
    {
        private const string BaseUrl = "/ui";

        private readonly ActivityTracker _tracker;

        public UiViewsModule(ActivityTracker tracker) 
            : base(BaseUrl)
        {
            _tracker = tracker;
            Get["/"] = _ => View["views/status.sshtml"];
            Get["/odaqueries"] = _ => View["views/odaqueries.sshtml"];
            Get["/status"] = _ => View["views/status.sshtml"];
            Get["/activity"] = _ => View["views/activity.sshtml"];
            Get["/gallery"] = _ => View["views/gallery.sshtml"];
            Get["/configure"] = _ => View["views/configure.sshtml"];
            Get["/about"] = _ => View["views/about.sshtml"];
            Get["/tools/diagnostics"] = _ => View["views/diagnostics.sshtml"];
            Get["/tools/sendnotification"] = _ =>
            {
                var siteInfo = _tracker.StartEvent;

                var config = Container.Resolve<WebServiceActivityConfig>();

                var notification = new NotificationEvent
                {
                    AgentId = siteInfo.AgentId,
                    CheckId = "CheckId",
                    CriticalFailure = false,
                    CriticalFailureDetails = null,
                    Data = "JSON Serialised copy of this...",
                    MinuteBucket = AnalyticsBaseline.MinuteBucket,
                    HourBucket = AnalyticsBaseline.HourBucket,
                    DayBucket = AnalyticsBaseline.DayBucket,
                    DisplayUnit = "",
                    EventType = "",
                    GeneratedOnUtc = DateTime.UtcNow,
                    Id = Guid.NewGuid(),
                    Latitude = "",
                    Longitude = "",
                    Message = "Message",
                    Properties = new Properties { {"SomeProperty", "SomeValue"} },
                    Result = false,
                    ResultCount = 0,
                    SiteId = siteInfo.SiteId,
                    State = MessageStateTypes.NotSet,
                    Tags = new List<string> {"SomeTag"},
                    Version = Guid.Empty
                };
                var keys = config.ApiKeys != null ? config.ApiKeys.ToArray() : new string[0];
                var model = new SendNotificationModel(notification, keys);
                return View["views/sendnotification.sshtml", model];
            };
        }
    }
}