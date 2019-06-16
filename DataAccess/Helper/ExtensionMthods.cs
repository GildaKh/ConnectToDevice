using DataAccess.Key;
using DeviceServer.Helper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectToDevice.Infrastructure.Helper
{
    public static class ExtensionMthods
    {
        public static bool CheckMac(this Image value, byte[] raw)
        {
            // var b = ConversionUtility.ImageToByteArray(value);
            var tmp = new byte[50000];

            Array.Copy(raw, 0, tmp, 0, raw.Length - 8);

            byte[] mac = ConversionUtility.CalculateMac(tmp, Keys.KeyImage);
            for (int i = 0; i < 8; i++)
            {
                if (raw[raw.Length - 8 + i] != mac[i])
                    return false;
            }
            return true;
        }
    }
}
