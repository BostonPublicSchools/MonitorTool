using System;
using System.Runtime.Serialization;

namespace Wolfpack.Core.WebServices.Interfaces.Exceptions
{
    public class CommunicationException : ApplicationException
    {
        public CommunicationException()
        {
        }

        public CommunicationException(string message) : base(message)
        {
        }

        protected CommunicationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public CommunicationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}