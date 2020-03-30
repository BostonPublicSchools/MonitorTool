namespace Wolfpack.Core.Interfaces
{
    /// <summary>
    /// This interface is used for HealthChecks to be loaded directly from the container.
    /// This is the recommended interface for HealthChecks to use
    /// </summary>
    public interface IHealthCheckPluginEx : IHealthCheckPlugin, ICanBeSwitchedOff, ISupportNotificationMode
    {
    }
}