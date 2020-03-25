using System.Collections.Generic;
using System.Linq;
using Wolfpack.Core.WebServices.Interfaces;
using Wolfpack.Core.WebServices.Interfaces.Entities;

namespace Wolfpack.Core.WebUI
{
    public class MenuChanger
    {
        private readonly IEnumerable<INeedMenuSpace> _menuChangers;
        private readonly IMenuMarkupBuilder _markupBuilder;

        public MenuChanger(IEnumerable<INeedMenuSpace> menuChangers,
            IMenuMarkupBuilder markupBuilder)
        {
            _menuChangers = menuChangers;
            _markupBuilder = markupBuilder;
        }

        public string BuildMarkup()
        {
            var requests = BuildChanges();
            return _markupBuilder.Build(requests);
        }
        private IEnumerable<MenuItem> BuildChanges()
        {
            var menus = new List<MenuItem>(BuildAddOnMenu());
            return menus;
        }

        private IEnumerable<MenuItem> BuildAddOnMenu()
        {
            var menuRequests = _menuChangers.Select(x => new
            {
                StartMenu = new MenuBuilder(),
                MenuConfigurer = x.Configure()
            }).ToList();

            var menuItems = menuRequests.SelectMany(rq =>
            {
                rq.MenuConfigurer(rq.StartMenu);
                return rq.StartMenu.Build();
            });

            var addons = new MenuBuilder();
            addons.AddDropdown("Add-Ons").AddItems(menuItems);
            return addons.Build();
        }
    }
}