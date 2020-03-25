
using System.Collections.Generic;
using System.Linq;

namespace Wolfpack.Core.WebServices.Interfaces.Entities
{
    //public class Test : INeedMenuSpace
    //{
    //    public Action<IMenuBuilder> Configure()
    //    {
    //        return menu => menu
    //            .AddDropdown("Sub-Drop")
    //            .AddItem("Item1", "/dashboard")
    //            .AddDivider()
    //            .AddItem("Item2", "/ui/item2")
    //            .Up()
    //            .AddItem("Item3", "/ui/item3");
    //    }
    //}

    public class MenuBuilder : IMenuBuilder
    {
        private readonly IList<IMenuItemBuilder> _builders;

        public MenuBuilder()
        {
            _builders = new List<IMenuItemBuilder>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <remarks>not supported in bootstrap v3 but used intenally to build top level dropdowns</remarks>
        public IDropDownMenuBuilder<IMenuBuilder> AddDropdown(string name)
        {
            var dropdownBuilder = new DropDownMenuItemBuilder<IMenuBuilder>(this, name);
            _builders.Add(dropdownBuilder);
            return dropdownBuilder;
        }

        public IMenuBuilder AddItem(string name, string url, string target = null)
        {
            _builders.Add(new MenuItemBuilder(name, url, target));
            return this;
        }

        public IMenuBuilder AddDivider()
        {
            _builders.Add(new PassThruMenuItemBuilder(MenuItem.Divider()));
            return this;
        }

        public IEnumerable<MenuItem> Build()
        {
            return _builders.SelectMany(x => x.Build());
        }
    }
}