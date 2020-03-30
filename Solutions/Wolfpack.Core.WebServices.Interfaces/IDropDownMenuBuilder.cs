using System.Collections.Generic;
using Wolfpack.Core.WebServices.Interfaces.Entities;

namespace Wolfpack.Core.WebServices.Interfaces
{
    public interface IDropDownMenuBuilder<out TOwner>
    {
        IDropDownMenuBuilder<TOwner> AddItem(MenuItem item);
        IDropDownMenuBuilder<TOwner> AddItem(string name, string url);
        IDropDownMenuBuilder<TOwner> AddItem(string name, string url, string target);
        IDropDownMenuBuilder<TOwner> AddItems(IEnumerable<MenuItem> items);
        IDropDownMenuBuilder<TOwner> AddDivider();
        TOwner Up();
    }
}