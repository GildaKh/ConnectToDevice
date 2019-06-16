using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectToDevice.Infrastructure.Helper
{
    internal static class ConfigUtility
    {
        internal static int MaxLLCharLength
        {
            get
            {
                try
                {
                    return Int32.Parse(ConfigurationManager.AppSettings["MaxLLCharLength"]);
                }
                catch (Exception)
                {
                    return 99;
                }
            }
        }
        internal static int MinPasswordLength
        {
            get
            {
                try
                {
                    return Int32.Parse(ConfigurationManager.AppSettings["MinPasswordLength"]);
                }
                catch (Exception)
                {
                    return 10;
                }
            }
        }
        internal static int MaxNumberLength
        {
            get
            {
                try
                {
                    return Int32.Parse(ConfigurationManager.AppSettings["MaxNumberLength"]);
                }
                catch (Exception)
                {
                    return 5;
                }
            }
        }
    }
}
