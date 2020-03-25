namespace Wolfpack.Core.Geo
{
    using Interfaces;
    using Interfaces.Entities;

    /// <summary>
    /// This will return a preset geo data object. It does this by intecepting all
    /// HealthCheckData messages
    /// </summary>
    public class StaticGeoLocator : IGeoLocator, IStartupPlugin
    {
        protected readonly GeoData _location;

        public StaticGeoLocator(AgentConfiguration config)
        {
            _location = !IsGeoSet(config.Latitude, config.Longitude)
                ? null
                : new GeoData
                       {
                           Latitude = config.Latitude,
                           Longitude = config.Longitude
                       };
        }

        private bool IsGeoSet(string latitude, string longitude)
        {
            return !((string.IsNullOrWhiteSpace(longitude) || string.IsNullOrWhiteSpace(latitude)));
        }

        public GeoData Locate()
        {
            return _location;
        }

        private void SetMessageGeoLocation(NotificationEvent message)
        {
            // if the geo data has not already been set by the health check
            if (!IsGeoSet(message.Latitude, message.Longitude))
            {
                message.Latitude = _location.Latitude;
                message.Longitude = _location.Longitude;
            }                
        }

        public Status Status { get; set; }

        /// <summary>
        /// Hooks into messenger pipeline to intercept all 
        /// HealthCheckData messages ONLY if it has been configured
        /// </summary>
        public void Initialise()
        {
            if (Locate() == null)
                return;

            Messenger.InterceptBefore<NotificationEvent>(SetMessageGeoLocation);
        }
    }
}