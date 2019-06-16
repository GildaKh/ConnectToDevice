using ConnectToDevice.Infrastructure.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectToDevice.Model.Interfaces
{
    public delegate bool DataReceivedDelegate(byte[] input);
    internal abstract class BaseChannel
    {       
        internal event DataReceivedDelegate DataReceived;

        protected virtual void OnDataReceived(byte[] input)
        {
            DataReceivedDelegate handler = DataReceived;
            if (handler != null)
            {
                handler(input);
            }
        }
        internal abstract void Connect();
        internal abstract bool Disconnect();
        internal abstract bool send(byte[] sendData);
        internal abstract bool IsConnected();
    }
}
