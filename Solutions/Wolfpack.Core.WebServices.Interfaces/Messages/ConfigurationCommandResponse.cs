using System;

namespace Wolfpack.Core.WebServices.Interfaces.Messages
{
    public class ConfigurationCommandResponse
    {
        public Exception Error { get; set; }
        public bool Result { get; set; }
    }
}