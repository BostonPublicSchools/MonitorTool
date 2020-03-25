using System.Collections.Generic;

namespace Wolfpack.Core.Interfaces.Entities
{
    public class NameValuePair
    {
        public string Name { get; set; } 
        public string Value { get; set; }

        public NameValuePair()
        {
            
        }

        public NameValuePair(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public NameValuePair(KeyValuePair<string, string> item)
            : this(item.Key, item.Value)
        {
        }
    }
}