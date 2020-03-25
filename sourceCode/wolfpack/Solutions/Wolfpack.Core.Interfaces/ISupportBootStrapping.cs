namespace Wolfpack.Core.Interfaces
{
    public interface ISupportBootStrapping<T>
    {
        void Execute(T config);
    }
}