using ConnectToDevice.Infrastructure.Helper;
using System;
using System.Text;
using System.IO;
using DeviceServer.Helper;

namespace ConnectToDevice.Model.Types
{
    internal class ILNUMBER : TypePackager<int>
    {
        internal ILNUMBER(int len, int value)
        {
            CheckLength(len, ConfigUtility.MaxLLCharLength, 1);
            Value = value;
        }

        internal override byte[] Pack(string data)
        {
            byte[] pp = new byte[(data.Length + 1) >> 1];
            byte[] output = new byte[(((data.Length + 1) >> 1)) * 2];
            pp = ConversionUtility.Str2Bcd(data, false, pp, 0);
            byte[] len = Encoding.GetEncoding(1256).GetBytes(pp.Length.ToString());       //len data
            var startAt = 2 - len.Length;
            Buffer.BlockCopy(len, 0, output, startAt, len.Length);                        //copy len data to beginning of output
            Buffer.BlockCopy(pp, 0, output, 2, pp.Length);                                //copy data to output

            return output;
        }
    }
}