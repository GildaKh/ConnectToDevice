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
    internal class ILPASSWORD : TypePackager<string>
    {
        internal ILPASSWORD(int len, string value)
        {
            CheckLength(len, ConfigUtility.MaxLLCharLength, 5);
            Value = value;
        }

        internal override byte[] Pack(string data)
        {
            var encryptedData = ConversionUtility.Sha256(data);
            byte[] output = ConversionUtility.StringToByteWithLen(2, encryptedData);                                         //copy data to output
            return output;
        }
    }
}
