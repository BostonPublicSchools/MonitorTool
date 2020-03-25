using System;
using System.Runtime.Serialization;

namespace Wolfpack.Core.WebServices.Interfaces.Exceptions
{
    public class StaleMessageException : ApplicationException
    {
        public StaleMessageException()
        {
        }

        public StaleMessageException(string message) : base(message)
        {
        }

        protected StaleMessageException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public StaleMessageException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public StaleMessageException(Guid id)
            : this(string.Format("Message '{0}' is stale", id))
        {            
        }
    }
}