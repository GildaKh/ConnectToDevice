using ConnectToDevice.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ConnectToDevice.Model.Channel
{
    internal class NetworkChannel : BaseChannel
    {
        protected string Host;
        protected int Port;

        Socket socket;
        private byte[] Buffer = new byte[50000];

        internal NetworkChannel(string host, int port)
        {
            Host = host;
            Port = port;
        }

        internal override void Connect()
        {
            if (string.IsNullOrEmpty(Host) || Port <= 0)
                return;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(Host, Port);
            AcceptCallback();
        }
        public void AcceptCallback()
        {
            if (socket != null)
                socket.BeginReceive(Buffer, 0, Buffer.Length, 0, ReadCallback,
                            socket);

        }
        public void ReadCallback(IAsyncResult ar)
        {
            int bytesRead = 0;
            try
            {
                if (!IsConnected())
                    return;

                Socket s1 = (Socket)ar.AsyncState;
                bytesRead = s1.EndReceive(ar);

                if (bytesRead == 0)
                {
                    AcceptCallback();
                    return;
                }
                byte[] rawMsg = new byte[bytesRead];
                Array.Copy(Buffer, 0, rawMsg, 0, bytesRead);
                OnDataReceived(rawMsg);
                AcceptCallback();
            }
            catch (Exception ex)
            {
                return;
            }
        }
        internal override bool Disconnect()
        {
            try
            {
                if (socket != null)
                {
                    socket.Close();
                    socket = null;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        internal override bool IsConnected()
        {
            try
            {
                if (socket == null)
                    return false;

                var res = socket != null && !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
                return res;
            }
            catch (SocketException) { return false; }
        }
        internal override bool send(byte[] b)
        {
            try
            {
                if (!IsConnected())
                    throw new Exception("unconnected Channel");

                socket.BeginSend(b, 0, b.Length, SocketFlags.None, new AsyncCallback(CompleteMessageSending), null);
                return true;
            }
            catch (Exception)
            {
                Disconnect();
                return false;
            }
        }

        private void CompleteMessageSending(IAsyncResult ar)
        {
            var client = ar.AsyncState as Socket;
            if (client != null)
                client.EndSend(ar);
        }
    }
}
