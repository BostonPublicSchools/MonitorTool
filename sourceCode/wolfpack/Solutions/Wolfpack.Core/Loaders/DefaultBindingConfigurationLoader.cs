using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Castle.Core.Internal;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;
using System.Linq;

namespace Wolfpack.Core.Loaders
{
    public class DefaultBindingConfigurationLoader : ILoader<BindingConfiguration>
    {
        public bool Load(out BindingConfiguration[] components)
        {
            BindingConfiguration[] configs;
            var items = new List<BindingConfiguration>();

            if (ContainerLoader<BindingConfiguration>.Resolve(out configs))
                items.AddRange(configs);

            // now try to build binding configurations with convention
            // any subfolder of the config\checks folder is assumed to 
            // the name of a schedule component.
            var folders = Directory.GetDirectories(SmartLocation.GetLocation(@"config\checks"));

            folders.ForEach(folder =>
                                {
                                    // assume each file is a health check config file.
                                    // parse it for component ids
                                    var files = from file in Directory.GetFiles(folder, "*.config",
                                                                                SearchOption.TopDirectoryOnly)
                                                select file;

                                    var dirInfo = new DirectoryInfo(folder);
                                    var scheduleId = dirInfo.Name;

                                    files.ForEach(file =>
                                                      {
                                                          string[] ids;

                                                          if (ParseConfigFile(file, out ids))
                                                          {
                                                              ids.ForEach(checkId => items.Add(new BindingConfiguration
                                                                                              {
                                                                                                  HealthCheckConfigurationName = checkId,
                                                                                                  ScheduleConfigurationName = scheduleId
                                                                                              }));
                                                          }
                                                      });
                                });

            components = items.ToArray();
            return (components.Length > 0);
        }

        protected virtual bool ParseConfigFile(string file, out string[] ids)
        {
            var xdoc = XDocument.Load(file);
            var attributes = xdoc.Root.Descendants("component").Attributes("id").Select(at => at.Value);
            ids = attributes.ToArray();
            return (ids.Length > 0);
        }

        public bool Load(out BindingConfiguration[] components, Action<BindingConfiguration> action)
        {
            if (!Load(out components))
                return false;

            components.ForEach(action);
            return true;
        }
    }
}