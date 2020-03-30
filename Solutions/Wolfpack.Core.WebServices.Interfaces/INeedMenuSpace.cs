using System;
using Wolfpack.Core.WebServices.Interfaces.Entities;

namespace Wolfpack.Core.WebServices.Interfaces
{
    public interface INeedMenuSpace
    {
        Action<IMenuBuilder> Configure();
    }
}