using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ConnectToDevice.Infrastructure.Helper;
using DeviceServer.Helper;

namespace ConnectToDevice.Model.Types
{
    internal class LLCHAR : TypePackager<string>
    {
        internal LLCHAR(int len, string value)
        {
            CheckLength(len, ConfigUtility.MaxLLCharLength, 1);
            Value = value;
        }

        internal override byte[] Pack(string data)
        {
            byte[] output = ConversionUtility.StringToByteWithLen(2, data);
            return output;
        }
    }
}
