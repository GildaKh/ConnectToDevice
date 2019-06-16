using System;
using System.Configuration;

namespace DeviceServer.Helper
{
    internal static class ConfigUtility
    {
        internal static string ConnectionString
        {
            get
            {
                try
                {
                    return ConfigurationManager.ConnectionStrings["Dbconnection"].ConnectionString;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }
}
