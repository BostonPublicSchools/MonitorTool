using System;
using System.Collections.Generic;

namespace Wolfpack.Core.Interfaces.Entities
{
    /// <summary>
    /// Simple name/value dictionary to hold result properties
    /// </summary>
    public class Properties : Dictionary<string, string>
    {
        public Properties()
            : base(StringComparer.OrdinalIgnoreCase)
        {            
        }

        public Properties(Dictionary<string, string> items)
            : base(items, StringComparer.OrdinalIgnoreCase)
        {            
        }
    }
}