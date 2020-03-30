using System.Collections.Generic;
using Wolfpack.Core.WebServices.Interfaces.Entities;

namespace Wolfpack.Core.WebServices.Interfaces
{
    public interface IMenuItemBuilder
    {
        IEnumerable<MenuItem> Build();
    }
}