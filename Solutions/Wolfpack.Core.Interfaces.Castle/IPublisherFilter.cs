using Castle.DynamicProxy;

namespace Wolfpack.Core.Interfaces.Castle
{
    public interface IPublisherFilter : IInterceptor
    {
        string Publisher { get; set; }
        string Check { get; set; }
    }
}