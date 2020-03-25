using System;
using System.Collections.Generic;
using System.Diagnostics;
using Wolfpack.Core.Interfaces;
using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Pipeline
{
    public class DefaultPipeline<T> : IPipeline<T>
    {
        private IEnumerable<IPipelineStep<T>> _steps { get; set; }

        public DefaultPipeline(params IPipelineStep<T>[] steps)
        {
            _steps = steps;
        }

        public PipelineResult<T> Execute(T context)
        {
            var i = 0;
            var result = new PipelineResult<T>(context);           
            var timer = new Stopwatch();


            foreach (var step in _steps)
            {
                var stepResult = step.GenerateResult(i++);
                var continuation = ContinuationOptions.Continue;
                try
                {
                    timer.Start();
                    
                    step.PreValidate(context);

                    if (step.ShouldExecute(context))
                    {
                        continuation = step.Execute(context);
                        step.PostValidate(context);
                        stepResult.Succeeded();
                    }
                    else
                    {
                        stepResult.Skipped();
                    }
                }
                catch (Exception ex)
                {
                    stepResult.Failed(context, ex);
                    break;
                }
                finally
                {
                    stepResult.Duration = timer.Elapsed;
                    result.StepResults.Add(stepResult);

                    timer.Reset();
                }

                if (continuation == ContinuationOptions.Stop)
                    break;
            }

            return result;
        }
    }
}