using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.WebServices.Interfaces
{
    public interface IWebServiceReceiverStrategy
    {
        void Execute(NotificationEvent notification);
    }
}