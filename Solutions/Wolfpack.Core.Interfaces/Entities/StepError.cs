using System;

namespace Wolfpack.Core.Interfaces.Entities
{
    public class StepError<T>
    {
        public Exception Error { get; set; }
        public T State { get; set; }
    }
}