using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Wolfpack.Core.Interfaces.Entities
{
    [DebuggerDisplay("Result: {Success}")]
    public class PipelineResult<T>
    {
        public T Context { get; set; }

        public List<StepResult<T>> StepResults { get; set; }

        public PipelineResult(T context)
        {
            Context = context;
            StepResults = new List<StepResult<T>>();
        }

        public bool Success
        {
            get
            {
                if (StepResults == null)
                    return false;

                return StepResults.All(r => r.Failure == null);
            }
        }

        public TimeSpan Duration
        {
            get { return TimeSpan.FromMilliseconds(StepResults.Sum(r => r.Duration.TotalMilliseconds)); }
        }

        public virtual void Dump()
        {
            Console.WriteLine("Result: {0}", Success);
            Console.WriteLine("Duration: {0}ms", Duration.TotalMilliseconds);
            Console.WriteLine("Step Results:");
            StepResults.ForEach(s => Console.WriteLine("\t{0}", s));            
        }
    }
}