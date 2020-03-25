using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Interfaces
{
    public interface INotificationRequestFilter
    {
        string Mode { get; }
        bool Execute(NotificationRequest request);
    }
}