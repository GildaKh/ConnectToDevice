using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DeviceServer.Model
{
    public delegate void DataReceivedDelegate(PersonInfo person);
    public delegate void ImageFoundDelegate(ImageInfo image);

    internal class NetworkChannel
    {
        protected string ServerIpAddress;
        protected int Port;
        internal event DataReceivedDelegate DataReceived;
        private Socket _listener;
        private Socket _handler;

        protected virtual void OnDataReceived(PersonInfo e)
        {
            DataReceivedDelegate handler = DataReceived;
            if (handler != null)
            {
                handler(e);
            }
        }
        internal NetworkChannel(string host, int port)
        {
            Port = port;
            ServerIpAddress = host;
        }
        internal bool StartListening()
        {
            #region Changed IP
            try
            {
                if (_listener != null && _listener.LocalEndPoint != null && _listener.LocalEndPoint.ToString().Split(':')[0] != ServerIpAddress)
                {
                    CloseSocket();
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            #endregion

            if (_listener == null)
            {
                // Establish the local endpoint for the socket.
                IPAddress ipAddress = IPAddress.Parse(ServerIpAddress);
                var localEndPoint = new IPEndPoint(ipAddress, Port);

                // Create a TCP/IP socket.
                _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    _listener.Bind(localEndPoint);
                    _listener.Listen(1000);
                }
                catch (Exception e)
                {
                    return false;
                }
                ContinueListening();
            }
            return true;
        }
        private void ContinueListening()
        {
            _listener.BeginAccept(AcceptCallback, _listener);
        }
        internal void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                Socket listener = (Socket)ar.AsyncState;
                _handler = listener.EndAccept(ar);

                // Create the state object.  
                StateObject state = new StateObject();
                state.WorkSocket = _handler;
                _handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, ReadCallback, state);
            }
            catch (Exception)
            {
                Disconnect();
            }
        }
        internal void ReadCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            _handler = state.WorkSocket;
            int bytesRead = _handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                Receive(state.Buffer);
            }
        }

        internal bool Send(byte[] byteData)
        {
            return Send(_handler, byteData);
        }
        private bool Send(Socket state, byte[] byteData)
        {
            if (!IsConnected(state))
            {
                return false;
            }
            state.BeginSend(byteData, 0, byteData.Length, 0, SendCallback, state);
            return true;
        }
        private void SendCallback(IAsyncResult ar)
        {
            Socket state = (Socket)ar.AsyncState;
            int bytesSent = state.EndSend(ar);

        }
        private void CloseSocket()
        {
            try
            {
                if (_listener != null)
                {
                    //listener.Shutdown(SocketShutdown.Send);
                    _listener.Close();
                    _listener = null;
                }
            }
            catch (Exception ex)
            {

            }
        }
        private bool IsConnected(Socket state)
        {
            if (state == null)
                return false;
            var isConnected = !(state.Poll(1000, SelectMode.SelectRead) && state.Available == 0);
            return isConnected;
        }
        internal PersonInfo Receive(byte[] rawMsg)
        {
            PersonInfo info = new PersonInfo(); //ConversionUtility.Deserialize<PersonnelInfo>(rawMsg);
            var ret = info.Unpack(rawMsg);
            if (ret)
                OnDataReceived(info);
            return info;
        }
        internal bool Disconnect()
        {
            try
            {
                if (_listener != null)
                {
                    _listener.Close();
                    _listener = null;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public class StateObject
        {
            // Client  socket.  
            public Socket WorkSocket = null;
            // Size of receive buffer.  
            public const int BufferSize = 1024;
            // Receive buffer.  
            public byte[] Buffer = new byte[BufferSize];
        }
    }
}
