using System.Configuration;

namespace Wolfpack.Core.Database
{
    public class SmartConnectionString
    {
        public static string For(string connection)
        {
            return ConfigurationManager.ConnectionStrings[connection] != null
                       ? ConfigurationManager.ConnectionStrings[connection].ConnectionString
                       : connection;
        }
    }
}