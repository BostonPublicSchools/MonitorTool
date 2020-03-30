using System.Collections.Generic;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Interfaces
{
    public interface IConfigurationRepository
    {
        IEnumerable<ConfigurationEntry> Load();
        void Save(ConfigurationChangeRequest change);
    }
}