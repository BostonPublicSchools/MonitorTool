using System;
using System.IO;
using Wolfpack.Core;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;
using Wolfpack.Manager.Commands;

namespace Wolfpack.Manager
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (!File.Exists(SystemCommand.Filename))
                {
                    Logger.Warning("Command file '{0}' is missing - nothing to do...exiting!", SystemCommand.Filename);
                    Environment.ExitCode = -1;
                }

                ISystemCommand command;
                var instruction = Serialiser.FromXmlInFile<SystemCommand>(SystemCommand.Filename);

                if (instruction.RestartConsole != null)
                {
                    command = new RestartConsoleCommand(instruction.RestartConsole);
                }
                else if (instruction.RestartService != null)
                {
                    command = new RestartServiceCommand(instruction.RestartService);
                }
                else
                {
                    Logger.Warning("Wolfpack command file contains no instructions! :-S");
                    Environment.ExitCode = -2;
                    return;
                }

                command.Execute();
                File.Delete(SystemCommand.Filename);
                Environment.ExitCode = 0;
            }
            catch (Exception e)
            {
                Logger.Error(Logger.Event.During("Wolfpack.Manager").Encountered(e));
                Environment.ExitCode = -99;
            }
        }
    }
}
