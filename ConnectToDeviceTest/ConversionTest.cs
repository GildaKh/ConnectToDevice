using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DeviceServer.Helper;

namespace ConnectToDeviceTest
{
    [TestClass]
    public class ConversionTest
    {
        [TestMethod]
        public void ByteToIntNull()
        {
            int result = ConversionUtility.ByteToInt(null);
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void ByteToIntInvalid()
        {
            byte[] test = { 0x02, 0x0 };
            int result = ConversionUtility.ByteToInt(test);
            Assert.AreEqual(0, result);
        }
    }
}
