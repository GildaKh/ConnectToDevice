using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeviceServer.Helper;

namespace DeviceServer.Model
{

    public class ReceiveDataManagement
    {
        internal NetworkChannel Channel { get; set; }
        public event DataReceivedDelegate ResultReceived;
        public event ImageFoundDelegate ImageFound;

        public void Initialization(string ip, int port)
        {
            Channel = new NetworkChannel(ip, port);
            Channel.DataReceived += DataReceivedCompletely;
            Channel.StartListening();
        }

        public void DataReceivedCompletely(PersonInfo data)
        {
            ResultReceived(data);
            ImageInfo image = new ImageInfo();
            image = image.GetImage(data.Id);
            ImageFound(image);
            //return image;
            Dispose();
        }

        public bool Send(ImageInfo image)
        {
            if (Channel == null)
                return false;

            if (image == null || !image.IsSuccessful)
            {
                byte[] output = {0x01, 0x01};
                Channel.Send(output);
            }
            else 
            {
                byte[] result = { 0x00, 0x00 };
                byte[] imageByte = new byte[50000];
                int lenInt = image.ImageByte.Length;
                Array.Copy(image.ImageByte, imageByte, lenInt);
                byte[] mac = ConversionUtility.CalculateMac(imageByte, Keys.KeyImage);
                byte[] dest = new byte[12 + imageByte.Length + mac.Length];
                lenInt += 8;
                byte[] length = ConversionUtility.IntToByte(6, lenInt);
                var i = 6;
                Array.Copy(result, 0, dest, 0, 2);
                Array.Copy(length, 0, dest, 2, i);
                i += 2;
                Array.Copy(imageByte, 0, dest, i, lenInt - 8);
                i += lenInt - 8;
                Array.Copy(mac, 0, dest, i, 8);
                i += 8;
                var desc = ConversionUtility.StringToByteWithLen(4, image.Description);
                Array.Copy(desc, 0, dest, i, desc.Length);
                i += desc.Length;

                return Channel.Send(dest);
            }
            return false;
        }

        public void Dispose()
        {
            Channel.Disconnect();
        }
    }
}
