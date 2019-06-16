using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectToDevice.Model.Interfaces
{
    public interface IReceivedData
    {
        Image Image { get; set; }
        string Description { get; set; }
    }
}
