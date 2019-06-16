using ConnectToDevice.Model.Channel;
using ConnectToDevice.Model.Interfaces;
using ConnectToDevice.Model.TimerMngmt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using ConnectToDevice.Infrastructure.Helper;

namespace ConnectToDevice.Model
{
    public delegate void DataConvertedDelegate(ReceivedData data);

    internal class SendDataManagment : IChannel
    {
        #region Constructor
        private SendDataManagment(AsyncType async)
        {
            _asyncType = async;
            if (async == AsyncType.Async)
                _timer = new ManualTimerAsync();
            else if (async == AsyncType.Sync)
                _timer = new ManualTimerSync();

            _timer.InitTimer();
            _timer.Timeout += RaiseTimeout;
        }

        private void RaiseTimeout()
        {
            _receivedData = new ReceivedData()
            {
                Description = "No Message Received. Timeout"
            };
            if (_asyncType == AsyncType.Async)
                ResultReceived(_receivedData);
        }

        public SendDataManagment(AsyncType asyncType, string ip, int portName)
            : this(asyncType)
        {
            Channel = new NetworkChannel(ip, portName);
            Channel.DataReceived += ResultReceivedCompletely;
        }
        public SendDataManagment(bool reInitialization, AsyncType asyncType, int comPort, int boudRate)
            : this(asyncType)
        {
            Channel = new SerialChannel(reInitialization, "COM" + comPort, boudRate);
            Channel.DataReceived += ResultReceivedCompletely;
        }
        #endregion

        #region Interface Implementation
        BaseChannel IChannel.Channel
        {
            get
            {
                return Channel;
            }
        }
        ReceivedData IChannel.Send(SendData data)
        {
            return Send(data);
        }
        #endregion

        private ManualTimer _timer;
        private AsyncType _asyncType;
        private ReceivedData _receivedData;
        internal BaseChannel Channel { get; set; }

        internal event DataConvertedDelegate ResultReceived;

        private bool ResultReceivedCompletely(byte[] rawMsg)
        {
            try
            {
                if (_receivedData == null)
                    _receivedData = new ReceivedData();
                var ret = _receivedData.Unpack(rawMsg);
                if (!ret)
                    return false;
                if (ResultReceived != null)
                    ResultReceived(_receivedData);
                else
                    return false;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        internal ReceivedData Send(SendData data)
        {
            _timer.SetTimer(20);
            if (Channel == null)
                return null;

            Channel.Connect();
            var ret = Channel.send(data.Pack());
            if (!ret)
            {
                _receivedData = new ReceivedData()
                {
                    Description = "Connection Error"
                };
                if (_asyncType == AsyncType.Async && ResultReceived != null)
                {
                    ResultReceived(_receivedData);
                }
                else
                {
                    return _receivedData;
                }
            }
            if (_asyncType == AsyncType.Sync)
            {
                while (_receivedData != null && !_timer.EndofTimer)
                {
                    Thread.Sleep(100);
                }
                ////Timeout
                //if (_timer != null && _timer.EndofTimer)
                //{
                //    return new ReceivedData()
                //    {
                //        Description = "Timeout"
                //    };
                //}
                //Dispose();

                return _receivedData;
            }
            return null;
        }
        internal void Dispose()
        {
            Channel.Disconnect();
            _timer.UnSetTimer();
            _receivedData = null;
        }
    }
}
