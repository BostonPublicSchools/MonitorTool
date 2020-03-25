using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;
using System.Linq;

namespace Wolfpack.Core.Configuration
{
    public class DefaultConfigurationManager : IConfigurationManager
    {
        [DllImport("user32.dll")]
        static extern bool AllowSetForegroundWindow(IntPtr hWnd);


        private static readonly object SyncObj = new object();

        private readonly IList<ConfigurationChangeRequest> _pendingChanges;
        private readonly IEnumerable<IConfigurationRepository> _repositories;
        private readonly AgentConfiguration _agentInfo;
        private IEnumerable<ConfigurationEntry> _entries;
        private bool _restartPending;

        public IEnumerable<ConfigurationChangeRequest> PendingChanges
        {
            get { return _pendingChanges; }
        }

        public DefaultConfigurationManager(IEnumerable<IConfigurationRepository> repositories,
            AgentConfiguration agentInfo)
        {
            _repositories = repositories;
            _agentInfo = agentInfo;
            _pendingChanges = new List<ConfigurationChangeRequest>();
        }

        public void Initialise()
        {
            var entries = new List<ConfigurationEntry>();
            entries.AddRange(LoadConfiguration());
            entries.AddRange(LoadCatalogue());
            _entries = entries.ToList();
        }

        public ConfigurationCatalogue GetCatalogue(params string[] tags)
        {
            var items = _entries.Where(ce => ce.Tags.ContainsAny(tags));
            return new ConfigurationCatalogue
            {
                InstanceId = _agentInfo.InstanceId,
                Items = items, 
                Pending = _pendingChanges
            };
        }

        public void Save(ConfigurationChangeRequest update)
        {
            lock(SyncObj)
            {
                if (_restartPending)
                    throw new InvalidOperationException(string.Format("Restart in progress"));

                update.Entry.Tags.RemoveAll(SpecialTags.ThatShouldNotBePersisted);
                _pendingChanges.Add(update);
            }
        }

        public void ApplyPendingChanges(bool restart)
        {
            lock (SyncObj)
            {
                if (!PendingChanges.Any())
                    return;

                Parallel.ForEach(_repositories, repository => PendingChanges.ToList().ForEach(repository.Save));
                _pendingChanges.Clear();

                if (!restart)
                    return;

                HandleRestart();
            }
        }

        protected void HandleRestart()
        {
            _restartPending = true;

            SystemCommand command;
            var windowStyle = ProcessWindowStyle.Hidden;

            if (Environment.UserInteractive)
            {
                // console
                windowStyle = ProcessWindowStyle.Normal;
                command = new SystemCommand
                              {
                                  RestartConsole = new RestartConsoleInstruction
                                                       {
                                                           ProcessId = Process.GetCurrentProcess().Id
                                                       }
                              };

                Logger.Debug("Writing console restart instruction file");
            }
            else
            {
                // service
                command = new SystemCommand
                              {
                                  RestartService = new RestartServiceInstruction
                                                       {
                                                           ServiceName = "Wolfpack"
                                                       }
                              };
                Logger.Debug("Writing service restart instruction file");
            }

            Serialiser.ToXmlInFile(SystemCommand.Filename, command);

            Logger.Info("Launching Wolfpack Helper to manage system restart...");
            var managerProcess = Process.Start(new ProcessStartInfo("wolfpack.manager.exe")
                              {
                                  WindowStyle = windowStyle
                              });

            if (managerProcess == null)
                return;

            Logger.Debug("Calling AllowSetForegroundWindow() on manager process...");
            AllowSetForegroundWindow(managerProcess.MainWindowHandle);
        }

        public void DiscardPendingChanges()
        {
            lock (SyncObj)
            {
                _pendingChanges.Clear();    
            }            
        }

        public IEnumerable<TagCloudEntry> GetTagCloud()
        {
            return _entries.SelectMany(e => e.Tags).GroupBy(t => t)
                .Select(g => new TagCloudEntry
                                 {
                                     Tag = g.Key, 
                                     Count = g.Count()
                                 }).OrderBy(t => t.Tag);
        }

        private IEnumerable<ConfigurationEntry> LoadConfiguration()
        {
            return _repositories.SelectMany(repository => repository.Load())
                .Select(ce =>
                            {
                                ce.Tags.AddIfMissing("Running");
                                return ce;
                            })
                .ToList();
        }

        private IEnumerable<ConfigurationEntry> LoadCatalogue()
        {
            Type[] items;

            TypeDiscovery.Discover<ISupportConfigurationDiscovery>(out items);

            return items.Select(i =>
                                    {
                                        var target = (ISupportConfigurationDiscovery) Activator.CreateInstance(i);
                                        return target.GetConfigurationMetadata();
                                    }).ToList();
        }
    }
}