using System.Collections.Generic;
using System.Text;
using Wolfpack.Core.WebServices.Interfaces;
using Wolfpack.Core.WebServices.Interfaces.Entities;

namespace Wolfpack.Core.WebUI
{
    public class DefaultMenuMarkupBuilder : IMenuMarkupBuilder
    {
        public virtual string Build(IEnumerable<MenuItem> requests)
        {
            var sb = new StringBuilder();

            foreach (var menuItem in requests)
            {
                sb.Append(GetMarkup(menuItem));
            }

            return sb.ToString();
        }

        protected virtual string GetMarkup(MenuItem item)
        {
            switch (item.Type.ToLower())
            {
                case "dropdown":
                    return GetDropdownMarkup(item);

                case "item":
                    return GetItemMarkup(item);

                case "divider":
                    return GetDividerMarkup();
            }

            return string.Empty;
        }

        protected virtual string GetDropdownMarkup(MenuItem item)
        {
            /*
             <li class="dropdown">
				<a href="#" class="dropdown-toggle" data-toggle="dropdown">Tools <b class="caret"></b></a>
				<ul class="dropdown-menu">
					<li><a href="/ui/tools/diagnostics">Diagnostics</a></li>
					<li><a href="/ui/tools/sendnotification">Send Notification</a></li>
				</ul>
			</li>
             */
            var sb = new StringBuilder("<li class=\"dropdown\">");
            sb.AppendFormat("<a href=\"#\" class=\"dropdown-toggle\" data-toggle=\"dropdown\">{0} <b class=\"caret\"></b></a>", item.Name);
            sb.Append("<ul class=\"dropdown-menu\">");

            foreach (var childItem in item.Children)
            {
                sb.Append(GetMarkup(childItem));
            }

            sb.Append("</ul>");
            sb.Append("</li>");
            return sb.ToString();
        }

        protected virtual string GetItemMarkup(MenuItem item)
        {
            /*
             <li><a href="/ui/configure">Configure</a></li>
             */
            return string.Format("<li><a {0}href=\"{1}\">{2}</a></li>", 
                string.IsNullOrWhiteSpace(item.Target) ? string.Empty : "target=\"" + item.Target + "\" ",
                item.Url, item.Name);
        }

        protected virtual string GetDividerMarkup()
        {
            return string.Format("<li class=\"divider\"></li>");
        }
    }
}