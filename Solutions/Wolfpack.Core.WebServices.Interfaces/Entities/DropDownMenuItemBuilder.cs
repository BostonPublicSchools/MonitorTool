using System.Collections.Generic;
using System.Linq;

namespace Wolfpack.Core.WebServices.Interfaces.Entities
{

    public class MenuItemBuilder : IMenuItemBuilder
    {
        private readonly MenuItem _item;
        public MenuItemBuilder(string name, string url, string target = null)
        {
            _item = MenuItem.Item(name, url, target);
        }
        public IEnumerable<MenuItem> Build()
        {
            return new[] {_item};
        }
    }

    public class DropDownMenuItemBuilder<TOwner> : IDropDownMenuBuilder<TOwner>, IMenuItemBuilder
    {
        private readonly TOwner _parent;
        private readonly string _name;
        private readonly List<IMenuItemBuilder> _builders;

        public DropDownMenuItemBuilder(TOwner parent, string name)
        {
            _parent = parent;
            _name = name;
            _builders = new List<IMenuItemBuilder>();
        }

        IDropDownMenuBuilder<TOwner> IDropDownMenuBuilder<TOwner>.AddItem(MenuItem item)
        {
            _builders.Add(new PassThruMenuItemBuilder(item));
            return this;
        }

        IDropDownMenuBuilder<TOwner> IDropDownMenuBuilder<TOwner>.AddItem(string name, string url)
        {
            _builders.Add(new MenuItemBuilder(name, url));
            return this;
        }

        IDropDownMenuBuilder<TOwner> IDropDownMenuBuilder<TOwner>.AddItem(string name, string url, string target)
        {
            _builders.Add(new MenuItemBuilder(name, url, target));
            return this;
        }  

        IDropDownMenuBuilder<TOwner> IDropDownMenuBuilder<TOwner>.AddItems(IEnumerable<MenuItem> items)
        {
            _builders.Add(new PassThruMenuItemBuilder(items));
            return this;
        }

        IDropDownMenuBuilder<TOwner> IDropDownMenuBuilder<TOwner>.AddDivider()
        {
            _builders.Add(new PassThruMenuItemBuilder(MenuItem.Divider()));
            return this;
        }

        TOwner IDropDownMenuBuilder<TOwner>.Up()
        {
            return _parent;
        }

        public IEnumerable<MenuItem> Build()
        {
            return new[]
            {
                MenuItem.Dropdown(_name, _builders.SelectMany(x => x.Build()).ToArray())
            };
        }
    }
}