using System;
using System.Reflection;
using System.Threading.Tasks;
using Magnum.Pipeline;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.WebServices.Interfaces.Entities;

namespace Wolfpack.Core.WebServices
{
    public class SignalRActivity : IActivityPlugin, IConsumer<NotificationEvent>
    {
        private readonly SignalRActivityConfig _config;

        private IDisposable _appHost;

        public Status Status { get; set; }

        public SignalRActivity(SignalRActivityConfig config)
        {
            _config = config;
        }

        public void Initialise()
        {
            Logger.Info("Initialising Wolfpack SignalR host...", _config.BaseUrl);
            Messenger.Subscribe(this);
        }

        public PluginDescriptor Identity
        {
            get
            {
                return new PluginDescriptor
                             {
                                 Name = "Wolfpack SignalR Host",
                                 TypeId = new Guid("B672BE60-7D84-4E1C-A287-C387F5845F07"),
                                 Description = "Wolfpack SignalR Hosting Activity"
                             };
            }
        }

        public void Start()
        {
            Logger.Info("\t\tStarting Wolfpack SignalR host on {0}", _config.BaseUrl);

            _appHost = WebApp.Start<Startup>(_config.BaseUrl);
        }

        public void Stop()
        {
            if (_appHost != null)
                _appHost.Dispose();
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

        public void Consume(NotificationEvent message)
        {
            GlobalHost.ConnectionManager.GetHubContext<Notifications>().Clients.All.notify(message);
        }
    }

    public class Notifications : Hub
    {
        public override Task OnConnected()
        {
            Logger.Debug("SignalR Client connected!");
            return base.OnConnected();
        }
    }

    class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new SignalRContractResolver()
            };
            var serializer = JsonSerializer.Create(settings);
            GlobalHost.DependencyResolver.Register(typeof(JsonSerializer), () => serializer);

            app.Map("/signalr", map =>
            {
                // Setup the cors middleware to run before SignalR.
                // By default this will allow all origins. You can 
                // configure the set of origins and/or http verbs by
                // providing a cors options with a different policy.
                map.UseCors(CorsOptions.AllowAll);

                var hubConfiguration = new HubConfiguration
                {
                    // You can enable JSONP by uncommenting line below.
                    // JSONP requests are insecure but some older browsers (and some
                    // versions of IE) require JSONP to work cross domain
                    // EnableJSONP = true
                };

                // Run the SignalR pipeline. We're not using MapSignalR
                // since this branch is already runs under the "/signalr"
                // path.
                map.RunSignalR(hubConfiguration);
            });
        }
    }

    public class SignalRContractResolver : IContractResolver
    {
        private readonly Assembly _assembly;
        private readonly IContractResolver _camelCaseContractResolver;
        private readonly IContractResolver _defaultContractSerializer;

        public SignalRContractResolver()
        {
            _defaultContractSerializer = new DefaultContractResolver();
            _camelCaseContractResolver = new CamelCasePropertyNamesContractResolver();
            _assembly = typeof(Connection).Assembly;
        }

        #region IContractResolver Members

        public JsonContract ResolveContract(Type type)
        {
            if (type.Assembly.Equals(_assembly))
                return _defaultContractSerializer.ResolveContract(type);

            return _camelCaseContractResolver.ResolveContract(type);
        }

        #endregion
    }
}