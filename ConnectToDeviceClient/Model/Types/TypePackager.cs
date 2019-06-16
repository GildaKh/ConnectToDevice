using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectToDevice.Model.Types
{
    internal abstract class TypePackager<T>
    {
        internal void CheckLength(int len, int maxLength, int minLength)
        {
            if (len > maxLength)
            {
                throw new Exception("Length " + len + " too long for " + GetType().Name);
            }
            if (len < minLength)
            {
                throw new Exception("Length " + len + " too short for " + GetType().Name);
            }
        }
        internal T Value { get; set; }
        internal abstract byte[] Pack(string data);
    }
}
