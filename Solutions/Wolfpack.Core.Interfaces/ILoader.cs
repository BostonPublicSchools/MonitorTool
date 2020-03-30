using System;

namespace Wolfpack.Core.Interfaces
{
    public interface ILoader<TI>
    {
        bool Load(out TI[] components);
        bool Load(out TI[] components, Action<TI> action);
    }
}