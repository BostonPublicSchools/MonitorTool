using System.Collections.Generic;
using System.Linq;

namespace Wolfpack.Core.WebServices.Interfaces.Entities
{
    internal class PassThruMenuItemBuilder : IMenuItemBuilder
    {
        private readonly MenuItem[] _items;

        public PassThruMenuItemBuilder(params MenuItem[] items)
        {
            _items = items;
        }

        public PassThruMenuItemBuilder(IEnumerable<MenuItem> items)
        {
            _items = items.ToArray();
        }

        public IEnumerable<MenuItem> Build()
        {
            return _items;
        }
    }
}