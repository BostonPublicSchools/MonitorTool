using Wolfpack.Core;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Core.WebServices;
using Wolfpack.Core.WebServices.Extenders;
using Wolfpack.Core.WebServices.Interfaces;
using Wolfpack.Core.WebServices.Interfaces.Entities;

namespace Wolfpack.Tests.Activities
{
    public class WebServiceTestBootstrapper : IStartupPlugin
    {
        private readonly WebServiceActivityConfig _config;

        public WebServiceTestBootstrapper(WebServiceActivityConfig config)
        {
            _config = config;
        }

        public Status Status { get; set; }

        public void Initialise()
        {
            Container.RegisterInstance(_config);
            Container.RegisterInstance(new ActivityTracker());
            Container.RegisterAsSingleton<IWebServiceExtender>(typeof(CoreServicesExtender));
            Container.RegisterAsSingleton<IActivityPlugin>(typeof(WebServiceActivity));
            Container.RegisterAsSingleton<IWebServiceReceiverStrategy>(typeof(WebServiceTestReceiverStrategy));
        }
    }
}