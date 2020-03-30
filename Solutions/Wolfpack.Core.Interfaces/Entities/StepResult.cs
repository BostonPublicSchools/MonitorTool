using System;
using System.Text;

namespace Wolfpack.Core.Interfaces.Entities
{
    public enum StepResultTypes
    {
        Success = 0,
        Failed,
        Skipped
    }

    public class StepResult<T>
    {
        public int Index { get; set; }
        public StepResultTypes Status { get; set; }
        public TimeSpan Duration { get; set; }
        public StepError<T> Failure { get; set; }
        public string Name { get; set; }
       
        public StepResult(int index)
        {
            Index = index;
        }

        public StepResult<T> Skipped()
        {
            Status = StepResultTypes.Skipped;
            Failure = null;
            return this;
        }

        public StepResult<T> Succeeded()
        {
            Status = StepResultTypes.Success;
            Failure = null;
            return this;
        }

        public StepResult<T> Failed(T context, Exception ex)
        {
            Status = StepResultTypes.Failed;
            Failure = new StepError<T>
                          {
                              Error = ex,
                              State = context
                          };
            return this;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("[{0} '{1}'] => {2} in {3}ms",
                Index,
                Name,
                Status,
                Duration.TotalMilliseconds);

            if (Failure != null)
                sb.AppendFormat("; Details => {0}", Failure.Error.Message);

            return sb.ToString();
        }
    }
}