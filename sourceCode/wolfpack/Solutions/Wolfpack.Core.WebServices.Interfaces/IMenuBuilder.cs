
namespace Wolfpack.Core.WebServices.Interfaces
{
    public interface IMenuBuilder
    {
        IDropDownMenuBuilder<IMenuBuilder> AddDropdown(string name);
        IMenuBuilder AddItem(string name, string url, string target = null);
        IMenuBuilder AddDivider();
    }
}