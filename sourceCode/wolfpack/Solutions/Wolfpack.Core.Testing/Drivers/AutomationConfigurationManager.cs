using System.Collections.Generic;
using Wolfpack.Core.Configuration;
using Wolfpack.Core.Configuration.FileSystem;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Testing.Drivers
{
    public class AutomationConfigurationManager : IStartupPlugin
    {
        public Status Status { get; set; }

        public void Initialise()
        {
            var repositories = new List<IConfigurationRepository>
                                   {
                                       new FileSystemScheduleConfigurationRepository(@"TestData\Config\Schedules")
                                   };

            ConfigurationManager.Initialise(new DefaultConfigurationManager(repositories, new AgentConfiguration()));
        }
    }
}