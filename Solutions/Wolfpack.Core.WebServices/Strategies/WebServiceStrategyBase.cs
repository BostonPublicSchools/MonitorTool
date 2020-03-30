using Wolfpack.Core.Interfaces;

namespace Wolfpack.Core.WebServices.Strategies
{
    public abstract class WebServiceStrategyBase<TContext>
    {
        protected IPipelineStep<TContext> Load<TConcrete>()
            where TConcrete : IPipelineStep<TContext>
        {
            return Container.Resolve<IPipelineStep<TContext>, TConcrete>();
        }
    }
}