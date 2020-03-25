using Magnum.Pipeline;

namespace Wolfpack.Core.Interfaces.Magnum
{
    public interface IPublisherPlugin<T> : IPlugin, ICanBeSwitchedOff, IConsumer<T>
        where T: class
    {
        string FriendlyId { get; set; }
    }
}