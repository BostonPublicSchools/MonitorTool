using System;
using System.Collections.Generic;
using Nancy.Hosting.Self;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;
using System.Linq;
using Wolfpack.Core.WebServices.Interfaces;
using Wolfpack.Core.WebServices.Interfaces.Entities;

namespace Wolfpack.Core.WebServices
{
    public class WebServiceActivity : IActivityPlugin
    {
        private readonly WebServiceActivityConfig _config;
        private readonly ActivityTracker _tracker;
        private readonly IEnumerable<IWebServiceExtender> _extenders;

        private NancyHost _host;

        public Status Status { get; set; }

        public WebServiceActivity(WebServiceActivityConfig config, 
            ActivityTracker tracker,
            IEnumerable<IWebServiceExtender> extenders)
        {
            _config = config;
            _tracker = tracker;
            _extenders = extenders;

            _config.BaseUrl = _config.BaseUrl.TrimEnd('/');
            _config.BaseUrl += "/";
        }

        public void Initialise()
        {
            Logger.Info("Initialising Wolfpack WebService...");
            Logger.Debug("\t{0} WebService Extenders loaded...", _extenders.Count());

            foreach (var extender in _extenders)
                Logger.Debug("\t\t{0}", extender.GetType().Name);

            var nancyBootstrapper = new WolfpackNancyBootstrapper(_config, _extenders);

            _host = new NancyHost(new Uri(_config.BaseUrl), nancyBootstrapper,
                new HostConfiguration
                {
                    UrlReservations = new UrlReservations { CreateAutomatically = true }
                });

            _tracker.Start();
        }
        public PluginDescriptor Identity
        {
            get
            {
                return new PluginDescriptor
                             {
                                 Name = "Wolfpack WebService",
                                 TypeId = new Guid("9CBDBFFF-1CA7-44C1-9E58-DF02B9017905"),
                                 Description = "Wolfpack WebService interface, powered by NancyFx"
                             };
            }
        }

        public void Start()
        {
            Logger.Info("\t\tStarting Wolfpack WebService on {0}", _config.BaseUrl);
            _host.Start();
        }

        public void Stop()
        {
            Logger.Info("Stopping Wolfpack WebService...");
            _host.Stop();
            _host.Dispose();
            Logger.Info("Wolfpack WebService stopped");
        }

        public void Pause()
        {
            
        }

        public void Continue()
        {
            
        }

        public bool Enabled
        {
            get { return _config.Enabled; }
            set { _config.Enabled = value; }
        }
    }
}