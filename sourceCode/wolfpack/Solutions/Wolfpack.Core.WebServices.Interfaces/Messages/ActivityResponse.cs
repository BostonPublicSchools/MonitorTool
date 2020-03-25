using System.Collections.Generic;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.WebServices.Interfaces.Messages
{
    public class ActivityResponse
    {
        public List<NotificationEvent> CurrentNotifications { get; set; }
    }
}