using System;
using System.Linq;
using Nancy;
using Nancy.ModelBinding;
using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.WebServices.Interfaces;
using Wolfpack.Core.WebServices.Interfaces.Messages;

namespace Wolfpack.Core.WebServices.Modules
{
    public class NotificationModule : NancyModule
    {
        private const string BaseUrl = "/api/notification";

        private readonly IWebServiceReceiverStrategy _receiverStrategy;
        private readonly ActivityTracker _tracker;

        public NotificationModule(IWebServiceReceiverStrategy receiverStrategy,
            ActivityTracker tracker)
            : base(BaseUrl)
        {
            _tracker = tracker;
            _receiverStrategy = receiverStrategy;


            Post["/notify"] = request =>
            {
                var message = this.Bind<NotificationEvent>();
                message.GeneratedOnUtc = message.GeneratedOnUtc.ToUniversalTime();
                message.State = MessageStateTypes.Delivered;
                message.ReceivedOnUtc = DateTime.UtcNow;

                Logger.Info("Received Notification ({0}) {1}", message.EventType, message.Id);
                _receiverStrategy.Execute(message);

                return Response.AsJson(new { Message = "Success" }, HttpStatusCode.Accepted);
            };

            Get["/artifact/{NotificationId}"] = _ =>
            {
                var restRequest = this.Bind<HealthCheckArtifact>();
                var artifact = ArtifactManager.Get(restRequest.Name, restRequest.NotificationId);
                var response = Response.FromStream(artifact, restRequest.ContentType);
                response.Headers.Add("Content-Disposition", string.Format("attachment; filename=wolfpack-{0}.txt", restRequest.NotificationId));
                return response;
            };

            Get["/start"] = _ => Response.AsJson(new StatusResponse
            {
                Info = _tracker.StartEvent,
                Status = "Running"
            });

            Get["/list"] = _ => Response.AsJson(_tracker.Notifications.ToList());

            Get["/list/{TypeId}"] = _ =>
            {
                var restRequest = this.Bind<typeArtifact>();
                var returnData = _tracker.Notifications.Where(n => n.TypeId == restRequest.TypeId);
                var res = Response.AsJson(returnData.ToList());
                return res;
            };
        }
    }

    public class typeArtifact
    {
        public Guid TypeId { get; set; }
    }
}