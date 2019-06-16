using ConnectToDevice.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ConnectToDevice.Infrastructure.Helper;

namespace ConnectToDevice.Model
{
    [
       ComVisible(true),
       GuidAttribute("E34FF72F-040D-4EB7-BC9E-DE28ACF72EE8"),
       ComSourceInterfaces(typeof(IConnectToDeviceApiEventBase), typeof(IConnectToDeviceApi)),
       ClassInterface(ClassInterfaceType.None),
       ProgId("ConnectToDevice.ConnectToDeviceApi"),
   ]
    public class ConnectToDeviceApi : IDisposable
    {
        #region Events
        public event DataConvertedDelegate TransactionCompleted;
        #endregion

        #region Properties
        private AsyncType _asyncType;
        private SendDataManagment _sendData;
        #endregion
        public void Initialization(AsyncType asyncType, int comPort, int baudRate)
        {
            _asyncType = asyncType;

            if (_sendData == null)
            {
                _sendData = new SendDataManagment(true, asyncType, comPort, baudRate);
                _sendData.ResultReceived -= TransactionCompleted;
                _sendData.ResultReceived += TransactionCompleted;
            }
        }
        public void Initialization(AsyncType asyncType, string ip, int port)
        {
            _asyncType = asyncType;
            if (_sendData == null)
            {
                _sendData = new SendDataManagment(asyncType, ip, port);
                _sendData.ResultReceived -= TransactionCompleted;
                _sendData.ResultReceived += TransactionCompleted;
            }
        }
        public ReceivedData Send(SendData data)
        {
            if (_sendData == null)
            {
                Dispose();
                return null;
            }
            var receivedData = _sendData.Send(data);
            //Dispose();
            return receivedData;
        }
        public void Dispose()
        {
            _sendData.Dispose();
            _sendData = null;
        }
    }

    [
       ComVisible(true),
       GuidAttribute("43437E8F-B796-4BB5-8FBB-E34513202C91"),
       InterfaceType(ComInterfaceType.InterfaceIsIDispatch)
   ]
    public interface IConnectToDeviceApi
    {
        [DispId(100)]
        void Initialization(int timeout, AsyncType asyncType, int comPort, int baudRate);

        [DispId(101)]
        void Initialization(int timeout, AsyncType asyncType, string ip, int port);

        [DispId(102)]
        ReceivedData Send(SendData data);

        [DispId(103)]
        ReceivedData Dispose();
    }
    [
    ComVisible(true),
    Guid("F7BC7746-5351-4476-839B-312F411A63FD"),
    InterfaceType(ComInterfaceType.InterfaceIsIDispatch)
    ]
    public interface IConnectToDeviceApiEventBase
    {
        [DispId(1)]
        bool TransactionCompleted(ReceivedData result);
    }
}

