using System.Collections.Generic;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Configuration
{
    public class ConfigurationManager
    {
        private static IConfigurationManager _instance;

        public static void Initialise()
        {
            if (!Container.IsRegistered<IConfigurationManager>())
            {
                Container.RegisterAsSingleton<IConfigurationManager>(typeof(DefaultConfigurationManager));
            }

            Initialise(Container.Resolve<IConfigurationManager>());
        }

        public static void Initialise(IConfigurationManager instance)
        {
            _instance = instance;
            _instance.Initialise();
        }

        public static ConfigurationCatalogue GetCatalogue(params string[] tags)
        {
            return _instance.GetCatalogue(tags);
        }

        public static void Update(ConfigurationChangeRequest change)
        {
            _instance.Save(change);
        }

        public static IEnumerable<TagCloudEntry> GetTagCloud()
        {
            return _instance.GetTagCloud();
        }

        public static void ApplyPendingChanges(bool restart)
        {
            _instance.ApplyPendingChanges(restart);
        }
        public static void DiscardPendingChanges()
        {
            _instance.DiscardPendingChanges();
        }
    }
}