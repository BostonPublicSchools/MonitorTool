using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.WebServices.Interfaces.Messages
{
    public class StatusResponse
    {
        public NotificationEventAgentStart Info { get; set; }

        public string Status { get; set; }
    }
}