using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Artifacts
{
    public class ArtifactManagerBootstrapper : IStartupPlugin
    {
        public Status Status { get; set; }

        public void Initialise()
        {
            Container.RegisterAll<IArtifactFormatter>();

            if (!Container.IsRegistered<IArtifactManager>())
            {
                Container.RegisterInstance<IArtifactManager>(new FileSystemArtifactManager(
                                               SmartLocation.GetLocation("_artifacts"),
                                               Container.ResolveAll<IArtifactFormatter>()));
            }

            ArtifactManager.Initialise(Container.Resolve<IArtifactManager>());
        }
    }
}