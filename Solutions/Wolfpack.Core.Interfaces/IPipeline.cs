using Wolfpack.Core.Interfaces.Entities;

namespace Wolfpack.Core.Interfaces
{
    public interface IPipeline<T>
    {
        PipelineResult<T> Execute(T context);
    }
}