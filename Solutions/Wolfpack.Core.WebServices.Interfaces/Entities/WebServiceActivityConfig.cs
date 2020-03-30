using System.Collections.Generic;
using Wolfpack.Core.Interfaces;

namespace Wolfpack.Core.WebServices.Interfaces.Entities
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Information on how to secure with ssl cert
    /// http://blogs.msdn.com/b/jpsanders/archive/2009/09/29/walkthrough-using-httplistener-as-an-ssl-simple-server.aspx
    /// http://www.hanselman.com/blog/WorkingWithSSLAtDevelopmentTimeIsEasierWithIISExpress.aspx
    /// </remarks>
    public class WebServiceActivityConfig : ICanBeSwitchedOff
    {
        public string BaseUrl { get; set; }
        public bool Enabled { get; set; }
        public List<string> ApiKeys { get; set; }

        public WebServiceActivityConfig()
        {
            ApiKeys = new List<string>();
        }
    }
}