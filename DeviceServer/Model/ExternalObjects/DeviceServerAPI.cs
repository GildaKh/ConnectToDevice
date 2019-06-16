using System;
using System.Runtime.InteropServices;

namespace DeviceServer.Model
{
    public class ConnectToDeviceApi : IDisposable
    {
        #region Events
        public event DataReceivedDelegate DataReceived;
        #endregion

        private ReceiveDataManagement _receiveData;
        #region Properties
        #endregion
        public void Initialization(string ip, int port)
        {
            if (_receiveData == null)
            {
                _receiveData = new ReceiveDataManagement();
                _receiveData.ResultReceived -= DataReceived;
                _receiveData.ResultReceived += DataReceived;

                _receiveData.Initialization(ip, port);
            }
        }
        public bool Send(ImageInfo image)
        {
            if (_receiveData == null)
            {
                Dispose();
                return false;
            }
            var ret = _receiveData.Send(image);
            Dispose();

            return ret;
        }
        public void Dispose()
        {
            _receiveData.Dispose();
            _receiveData = null;
        }
    }
}

