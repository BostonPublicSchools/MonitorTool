using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Interfaces
{
    public enum ContinuationOptions
    {
        /// <summary>
        /// indicates that the execution should continue
        /// </summary>
        Continue = 0,
        /// <summary>
        /// indicates that the execution should stop and any subsequent steps need to be skipped
        /// </summary>
        Stop,
    }

    public interface IPipelineStep<T>
    {
        void PreValidate(T context);
        ContinuationOptions Execute(T context);
        void PostValidate(T context);

        bool ShouldExecute(T context);
        StepResult<T> GenerateResult(int index);
    }
}