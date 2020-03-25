using Nancy;
using Wolfpack.Core.WebServices.Interfaces.Messages;

namespace Wolfpack.Core.WebUI.Modules
{
    public class UiApiModule : NancyModule
    {
        private const string BaseUrl = "/ui/api";

        private readonly MenuChanger _menuChanger;

        public UiApiModule(MenuChanger menuChanger) 
            : base(BaseUrl)
        {
            _menuChanger = menuChanger;

            Get["/addonmenu"] = GetAddOnMenu;
        }

        private dynamic GetAddOnMenu(dynamic request)
        {
            var menuMarkup = _menuChanger.BuildMarkup();
            return Response.AsJson(new MenuChangeResponse { Markup = menuMarkup });
        }
    }
}