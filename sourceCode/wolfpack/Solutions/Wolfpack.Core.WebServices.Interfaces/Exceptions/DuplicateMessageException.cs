using System;
using System.Runtime.Serialization;

namespace Wolfpack.Core.WebServices.Interfaces.Exceptions
{
    public class DuplicateMessageException : ApplicationException
    {
        public DuplicateMessageException()
        {
        }

        public DuplicateMessageException(string message) : base(message)
        {
        }

        protected DuplicateMessageException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public DuplicateMessageException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public DuplicateMessageException(Guid id) 
            : this(string.Format("Message '{0}' is a duplicate", id))
        {            
        }
    }
}