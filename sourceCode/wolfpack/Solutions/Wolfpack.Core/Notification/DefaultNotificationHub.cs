using System.Collections.Generic;
using Castle.Core.Internal;
using Magnum.Pipeline;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.Notification.Filters.Request;

namespace Wolfpack.Core.Notification
{
    public class DefaultNotificationHub : INotificationHub, IConsumer<NotificationRequest>
    {
        private readonly AgentConfiguration _config;

        protected Dictionary<string, INotificationRequestFilter> _filters;

        public DefaultNotificationHub(AgentConfiguration config, IEnumerable<INotificationRequestFilter> filters)
        {
            _config = config;
            LoadFilters(filters);
        }

        public void Initialise()
        {
            // listen for request messages being published 
            // & route to the most appropriate filter
            Messenger.Subscribe(this);
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadFilters(IEnumerable<INotificationRequestFilter> filters)
        {
            _filters = new Dictionary<string, INotificationRequestFilter>();            

            // could use filters.ToDictionary() but I want a much richer
            // error message to aide debugging in the case of duplicate
            // filters..
            filters.ForEach(filter =>
                                {
                                    if (_filters.ContainsKey(filter.Mode))
                                    {
                                        Logger.Info("Warning! A Notification Request Filter named '{0}' is already loaded; implementing type is '{1}'",
                                            filter.Mode, _filters[filter.Mode].GetType().Name);
                                        return;
                                    }

                                    _filters.Add(filter.Mode.ToLower(), filter);
                                });

            Logger.Info("Loaded {0} Notification Request filters...", _filters.Count);
            _filters.ForEach(filter => Logger.Debug("\t'{0}' => {1}.{2}", filter.Key, 
                filter.Value.GetType().Namespace,
                filter.Value.GetType().Name));
        }

        public void Consume(NotificationRequest request)
        {
            Enrich(request);

            var filter = SelectFilter(request);
            Logger.Debug("Notification '{0}' matched Filter '{1}'", request.CheckId, filter.Mode);

            if (filter.Execute(request))
            {
                var notification = ConvertRequestToEvent(request);
                PublishEvent(notification);
            }
        }

        protected virtual void Enrich(NotificationRequest request)
        {
            if (request.Notification == null)
                return;

            if (string.IsNullOrWhiteSpace(request.Notification.AgentId))
                request.Notification.AgentId = _config.AgentId;
            if (string.IsNullOrWhiteSpace(request.Notification.SiteId))
                request.Notification.SiteId = _config.SiteId;
        }

        public virtual NotificationEvent ConvertRequestToEvent(NotificationRequest request)
        {
            var coreEvent = request.Notification;

            return new NotificationEvent
                       {
                           AgentId = coreEvent.AgentId,
                           CheckId = coreEvent.CheckId,
                           CriticalFailure = coreEvent.CriticalFailure,
                           CriticalFailureDetails = coreEvent.CriticalFailureDetails,
                           DayBucket = coreEvent.DayBucket,
                           EventType = coreEvent.EventType,
                           GeneratedOnUtc = coreEvent.GeneratedOnUtc,
                           HourBucket = coreEvent.HourBucket,
                           Id = coreEvent.Id,
                           Latitude = coreEvent.Latitude,
                           Longitude = coreEvent.Longitude,
                           Message = coreEvent.Message,
                           MinuteBucket = coreEvent.MinuteBucket,
                           Properties = coreEvent.Properties,
                           ReceivedOnUtc = coreEvent.ReceivedOnUtc,
                           Result = coreEvent.Result,
                           ResultCount = coreEvent.ResultCount,
                           DisplayUnit = coreEvent.DisplayUnit,
                           SiteId = coreEvent.SiteId,
                           State = coreEvent.State,
                           Tags = coreEvent.Tags,
                           Version = coreEvent.Version,
                           Data = Serialiser.ToJson(coreEvent),
                           TypeId = request.Notification.TypeId
                       };
        }

        public virtual void PublishEvent(NotificationEvent notification)
        {
            Messenger.Publish(notification);
        }

        public virtual INotificationRequestFilter SelectFilter(NotificationRequest request)
        {
            var key = (request.Mode ?? string.Empty).ToLowerInvariant();
            return _filters.ContainsKey(key) ? _filters[key] : new AlwaysPublishNotificationFilter();            
        }
    }
}