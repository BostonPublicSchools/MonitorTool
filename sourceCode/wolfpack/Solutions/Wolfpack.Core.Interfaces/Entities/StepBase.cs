namespace Wolfpack.Core.Interfaces.Entities
{
    public abstract class StepBase<T> : IPipelineStep<T>
    {
        public abstract ContinuationOptions Execute(T context);

        public virtual void PreValidate(T context)
        {            
        }

        public virtual void PostValidate(T context)
        {            
        }

        public virtual bool ShouldExecute(T context)
        {
            return true;
        }

        public virtual StepResult<T> GenerateResult(int index)
        {
            return new StepResult<T>(index) { Name = GetType().Name };
        }
    }
}