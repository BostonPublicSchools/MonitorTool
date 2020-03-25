using System;

namespace Wolfpack.Core.Interfaces.Entities
{
    public class AgentConfiguration
    {
        private string _siteId;
        public string SiteId
        {
            get { return string.IsNullOrWhiteSpace(_siteId) ? Environment.MachineName : _siteId; }
            set { _siteId = value; }
        }

        public string AgentId { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public Guid InstanceId { get; set; }

        public AgentConfiguration()
        {
            InstanceId = Guid.NewGuid();
        }
    }
}