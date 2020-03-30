using System.Collections.Generic;
using ServiceStack.Web;

namespace Wolfpack.Core.WebServices.Interfaces.Messages
{
    public class Atom
    {

    }

    public class AtomResponse : IHasOptions
    {
        public IDictionary<string, string> Options
        {
            get
            {
                var opts = new Dictionary<string, string>();
                opts.Add("Content-Type", "application/atom+xml");
                return opts;
            }
        }
    }
}