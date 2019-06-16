using ConnectToDevice.Infrastructure.Helper;
using ConnectToDevice.Model.Interfaces;
using DeviceServer.Helper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectToDevice.Model
{

    public class ReceivedData : IReceivedData
    {
        public Image Image { get; set; }
        public string Description { get; set; }
        internal bool Unpack(byte[] input)
        {
            try
            {
                if (input[0] == 0x00 && input[1] == 0x00)
                {
                    var b = new byte[10];
                    var i = 6;
                    Array.Copy(input, 2, b, 0, i);
                    i += 2;
                    int imageLen = 0;
                    var len = Encoding.GetEncoding(1256).GetString(b).Replace("\0", "");
                    int.TryParse(len, out imageLen);
                    var tmp = new byte[imageLen];
                    Array.Copy(input, i, tmp, 0, imageLen);
                    using (var ms = new MemoryStream(tmp))
                    {
                        Image = Image.FromStream(ms);
                        if (!Image.CheckMac(tmp))
                            return false;
                    }
                    tmp = new byte[200];
                    Array.Copy(input, i + imageLen, tmp, 0, 4);
                    i += 4;
                    var j = ConversionUtility.ByteToInt(tmp);
                    Array.Copy(input, i + imageLen, tmp, 0, j);
                    Description = Encoding.GetEncoding(1256).GetString(tmp).Replace("\0", "");

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
