using System;
using System.Configuration;
using System.IO;
using Castle.Core.Resource;
using Castle.Windsor.Configuration.Interpreters;
using System.Linq;

namespace Wolfpack.Core.Containers
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>http://stackoverflow.com/questions/317981/can-castle-windsor-locate-files-in-a-subdirectory</remarks>
    public class ZeroAppConfigXmlInterpreter : XmlInterpreter
    {
        public override void ProcessResource(IResource source, 
            Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store, 
            Castle.MicroKernel.IKernel kernel)
        {
            // default stuff...
            base.ProcessResource(source, store, kernel);

            // custom stuff..auto register all config\*.castle.config files
            var configFilesLocation = SmartLocation.GetLocation("config");

            if (!Directory.Exists(configFilesLocation))
                return;

            ProcessFolder(store, kernel, configFilesLocation);
        }

        protected void ProcessFolder(Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store, 
            Castle.MicroKernel.IKernel kernel,
            string path)
        {
            foreach (var extraConfig in Directory.GetFiles(path, "*.castle.config"))
            {
                try
                {
                    var interpreter = new XmlInterpreter(extraConfig);
                    interpreter.ProcessResource(interpreter.Source, store, kernel);
                }
                catch (ConfigurationErrorsException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Failed to load configuration: " + extraConfig, ex);
                }
            }
            
            Directory.GetDirectories(path).ToList().ForEach(folder => ProcessFolder(store, kernel, folder));
        }
    }
}