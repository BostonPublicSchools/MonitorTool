using System.Collections.Generic;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Interfaces
{
    public interface IConfigurationManager
    {
        IEnumerable<ConfigurationChangeRequest> PendingChanges { get; }

        void Initialise();
        ConfigurationCatalogue GetCatalogue(params string[] tags);
        void Save(ConfigurationChangeRequest request);
        void ApplyPendingChanges(bool restart);
        void DiscardPendingChanges();
        IEnumerable<TagCloudEntry> GetTagCloud();
    }
}