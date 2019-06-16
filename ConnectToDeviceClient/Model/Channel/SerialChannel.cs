using ConnectToDevice.Infrastructure.Helper;
using ConnectToDevice.Model.Interfaces;
using ConnectToDevice.Model.TimerMngmt;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConnectToDevice.Model.Channel
{
    internal class SerialChannel : BaseChannel
    {
        protected SerialPort Port { get; set; }
        internal SerialChannel(bool reInitialization, string portName, int baudRate)
        {
            if (reInitialization)
                SetComPort(portName, baudRate);
        }
        internal SerialPort SetComPort(string comPort, int baudRate)
        {
            if (SerialPort.GetPortNames().Any(p => p.ToUpper() == comPort.ToUpper()))
            {
                PreparePort(comPort.ToUpper(), baudRate);
                try
                {
                    Port.Open();
                }
                catch (Exception ex)
                {
                    return null;
                }

                return Port;
            }
            return null;
        }
        private void PreparePort(string portName, int baudRate)
        {
            if (Port != null)
            {
                if (Port.IsOpen)
                    Port.Close();
                Port.Dispose();
                Port = null;
                GC.Collect();
            }

            Thread.Sleep(500);
            Port = new SerialPort
            {
                PortName = portName,
                BaudRate = baudRate,
                ReceivedBytesThreshold = 1,
                Parity = Parity.None,
                StopBits = StopBits.One,
                ReadTimeout = 3000,
                WriteTimeout = 3000,
                DtrEnable = true,
                RtsEnable = true,
                Handshake = Handshake.None
            };
            Port.DataReceived += Port_DataReceived;
            //ContinueWaiting = true;
        }
        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int bytesRead = 0;
            byte[] rawMsg;
            byte[] msg = new byte[50000];
            Thread.Sleep(300);
            try
            {
                if (!IsConnected())
                    return;

                bytesRead = Port.Read(msg, 0, Port.BytesToRead);

                if (bytesRead == 0)
                    return;
                rawMsg = new byte[bytesRead];
                Array.Copy(msg, rawMsg, bytesRead);
                OnDataReceived(rawMsg);

            }
            catch (Exception ex)
            {

            }
        }
        internal override void Connect()
        {
        }
        internal override bool Disconnect()
        {
           try
            {
                if (Port != null)
                {
                    if (Port.IsOpen)
                        Port.Close();
                    Port.Dispose();
                    Port = null;
                    GC.Collect();
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        internal override bool IsConnected()
        {
            return Port != null && Port.IsOpen;
        }
        internal override bool send(byte[] b)
        {
            try
            {
                if (!IsConnected())
                    throw new Exception("unconnected Channel");

                Port.Write(b, 0, b.Length);
                return true;
            }
            catch (Exception)
            {
                Disconnect();
                return false;
            }
        }
    }
}
