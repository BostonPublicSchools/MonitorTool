namespace Wolfpack.Core.Interfaces.Entities
{
    public class SystemCommand
    {
        public const string Filename = "_cmd.xml";

        public RestartConsoleInstruction RestartConsole { get; set; }
        public RestartServiceInstruction RestartService { get; set; }
    }
}