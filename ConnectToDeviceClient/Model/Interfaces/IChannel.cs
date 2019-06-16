using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectToDevice.Model.Interfaces
{
    internal interface IChannel
    {
        BaseChannel Channel { get;}
        ReceivedData Send(SendData data);
    }
}
