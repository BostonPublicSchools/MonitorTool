using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.WebServices.Interfaces.Entities
{
    public class WebServicePublisherConfig : PluginConfigBase
    {
        public const int DefaultSendInterval = 10;

        public string BaseFolder { get; set; }        
        public int? SendIntervalInSeconds { get; set; }
        public string UserAgent { get; set; }
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }
    }
}