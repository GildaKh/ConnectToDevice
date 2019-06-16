using ConnectToDevice.Model.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectToDevice.Model.Interfaces
{
    internal interface IPersonnel
    {
        LLCHAR Name { get; }
        LLCHAR FamilyName { get; }
        LLCHAR UserName { get; }
        ILPASSWORD Password { get;  }
        ILNUMBER Code { get;}
        byte[] Pack();
    }
}
