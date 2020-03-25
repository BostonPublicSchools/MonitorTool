
namespace Wolfpack.Core.Interfaces
{
    /// <summary>
    /// A profile enables rapid configuration of an agent. This involves
    /// loading the IoC container with the correct components
    /// </summary>
    public interface IRoleProfile
    {
        string Name { get; }
        IRolePlugin Role { get; }
    }
}