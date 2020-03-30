using System;
using System.ServiceProcess;
using System.Threading;
using Wolfpack.Core;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;
using System.Linq;

namespace Wolfpack.Manager.Commands
{
    public class RestartServiceCommand : ISystemCommand
    {
        private RestartServiceInstruction _instruction;

        public RestartServiceCommand(RestartServiceInstruction instruction)
        {
            _instruction = instruction;
        }

        public void Execute()
        {
            Logger.Info("Stopping Wolfpack service (ServiceName={0})...", _instruction.ServiceName);

            var wolfpack = ServiceController.GetServices().FirstOrDefault(s => 
                s.ServiceName.Equals(_instruction.ServiceName, StringComparison.OrdinalIgnoreCase));

            if (wolfpack == null)
                throw new InvalidOperationException("Wolfpack service not installed :-S");

            wolfpack.Stop();
            wolfpack.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));

            Logger.Info("Restarting Wolfpack...");
            wolfpack.Start();
        }
    }
}