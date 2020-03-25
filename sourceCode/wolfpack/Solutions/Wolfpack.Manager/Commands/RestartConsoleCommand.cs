using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using WindowsInput;
using WindowsInput.Native;
using Wolfpack.Core;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Manager.Commands
{
    public class RestartConsoleCommand : ISystemCommand
    {       
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        private readonly RestartConsoleInstruction _instruction;

        public RestartConsoleCommand(RestartConsoleInstruction instruction)
        {
            _instruction = instruction;
        }

        public void Execute()
        {
            try
            {
                Logger.Info("Locating wolfpack console, process id = {0}...", _instruction.ProcessId);
                var console = Process.GetProcessById(_instruction.ProcessId);

                Logger.Info("Process located...sending close message");
                Thread.Sleep(3000);

                var isFront = SetForegroundWindow(console.MainWindowHandle);

                if (!isFront)
                {
                    Logger.Warning("Unable to make Wolfpack console the foreground window, trying again...");
                    isFront = SetForegroundWindow(console.MainWindowHandle);

                    if (!isFront)
                    {
                        Logger.Warning("Still unable to make Wolfpack console the foreground window, please manually restart wolfpack!");

                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        return;
                    }
                }

                Thread.Sleep(1000);
                var sim = new InputSimulator();
                sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_C);

                Logger.Info("Close message sent, waiting for process to exit");
                WaitForApplicationToClose(console);
                StartApplication();
            }
            catch (Exception e)
            {
                Logger.Error(Logger.Event.During("RestartConsoleCommand.Execute").Encountered(e));
                throw;
            }
        }

        private void StartApplication()
        {
            Logger.Info("Starting wolfpack...");
            Process.Start(new ProcessStartInfo("wolfpack.agent.exe")
                              {
                                  CreateNoWindow = true
                              });
        }

        private static void WaitForApplicationToClose(Process console)
        {
            var i = 0;

            while (!console.HasExited && i++ < 20)
            {
                Thread.Sleep(1000);
            }

            if (!console.HasExited)
                throw new InvalidOperationException(string.Format("Unable to close wolfpack process :("));
        }
    }
}