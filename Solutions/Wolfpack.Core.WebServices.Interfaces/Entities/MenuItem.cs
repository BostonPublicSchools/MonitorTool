using System.Collections.Generic;

namespace Wolfpack.Core.WebServices.Interfaces.Entities
{
    public class MenuItem
    {
        private readonly List<MenuItem> _children;

        public string Type { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Target { get; set; }
        public IList<MenuItem> Children { get { return _children; }}

        public MenuItem()
        {
            _children = new List<MenuItem>();
        }

        public static MenuItem Item(string name, string url, string target = null)
        {
            return new MenuItem
            {
                Name = name,
                Type = "Item",
                Url = url,
                Target = target
            };
        }
        public static MenuItem Dropdown(string name, params MenuItem[] childItems)
        {
            var item = new MenuItem
            {
                Name = name,
                Type = "DropDown"
            }; 

            item._children.Clear();
            item._children.AddRange(childItems);
            return item;
        }

        public static MenuItem Divider()
        {
            return new MenuItem
            {
                Type = "Divider"
            };
        }
    }
}