using System;

namespace Wolfpack.Core.Interfaces
{
    public interface INow
    {
        DateTime Now();
        DateTime UtcNow();
    }
}