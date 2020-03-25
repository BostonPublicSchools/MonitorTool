using System.Collections.Generic;
using System.Linq;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.TinyIoc;
using Wolfpack.Core.WebServices.Interfaces;
using Wolfpack.Core.WebServices.Interfaces.Entities;

namespace Wolfpack.Core.WebServices
{
    public class WolfpackNancyBootstrapper : DefaultNancyBootstrapper
    {
        private readonly WebServiceActivityConfig _config;
        private readonly IEnumerable<IWebServiceExtender> _extenders;

        public WolfpackNancyBootstrapper(WebServiceActivityConfig config,
            IEnumerable<IWebServiceExtender> extenders)
        {
            _config = config;
            _extenders = extenders;
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            foreach (var serviceExtender in _extenders)
            {
                serviceExtender.Execute(container, pipelines);
            }
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
            container.Register(_config);
            container.Register((ctx, npo) => Container.Resolve<IWebServiceReceiverStrategy>());
            container.Register((ctx, npo) => Container.Resolve<ActivityTracker>());
        }

        protected override IEnumerable<ModuleRegistration> Modules
        {
            get { return _extenders.SelectMany(x => x.Modules); }
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);

            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("Scripts"));
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("fonts"));
        }
    }
}