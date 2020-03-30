using Wolfpack.Core.Interfaces;

namespace Wolfpack.Core.WebServices.Interfaces.Entities
{
    public class SignalRActivityConfig : ICanBeSwitchedOff
    {
        public string BaseUrl { get; set; }
        public bool Enabled { get; set; }         
    }
}