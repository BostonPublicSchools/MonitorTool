using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Interfaces
{
    /// <summary>
    /// NOTE: NO SUPPORT FOR THIS YET - JUST AN IDEA!
    /// This interface can optionally be implemented by an <see cref="IHealthCheckPlugin"/>
    /// component to read and write it's state immediately before and after it's invocation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHealthCheckStateManager<T>
    {
        T Load(PluginDescriptor check);
        void Save(PluginDescriptor check, T state);
    }
}