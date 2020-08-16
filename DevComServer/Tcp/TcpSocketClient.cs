using iFactory.CommonLibrary.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace iFactory.DevComServer
{
    /// <summary>
    /// Tcp通信参数
    /// </summary>
    public class TcpParmeters
    {
        public TcpParmeters()
        {
            this.MultiplyReceiveMode = false;
            this.Ratio = 1;
            this.DecimalNum = 1;
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 周期读取命令
        /// </summary>
        public string CycReadComand { set; get; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Ip { set; get; }
        /// <summary>
        /// 端口号
        /// </summary>
        public int Port { set; get; }
        /// <summary>
        /// 小数量位置
        /// </summary>
        public int DecimalNum { set; get; }
        /// <summary>
        /// 多次接收模式。每次发送命令，对方返回数据一次性接收不完，需要再次接收的
        /// </summary>
        public bool MultiplyReceiveMode { set; get; }
        /// <summary>
        /// 乘以的倍数
        /// </summary>
        public decimal Ratio { set; get; }
    }
    /// <summary>
    /// 接收对象
    /// </summary>
    public class TcpReceiveObject
    {
        // Client socket.  
        public Socket workSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 256;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder ReceiveInfo = new StringBuilder();
    }
    /// <summary>
    /// TCP连接标准接口
    /// </summary>
    public interface ITcpClient
    {
        bool Connect();
        void DisConnect();
        bool SendAndReceiveDataAsync(string Command, bool IsReceiveData = false);
    }
    /// <summary>
    /// 异步连接客户端
    /// </summary>
    public class AsynTcpClient:ITcpClient
    {
        // ManualResetEvent instances signal completion.  
        private ManualResetEvent connectDone = new ManualResetEvent(false);
        private ManualResetEvent sendDone = new ManualResetEvent(false);
        private ManualResetEvent receiveDone = new ManualResetEvent(false);
        public TcpParmeters socketCfg;
        private TcpReceiveObject socketReceiveObject;
        public Socket socketClient;
        private readonly ILogWrite _log;

        public AsynTcpClient(TcpParmeters Config, ILogWrite logWrite, bool IsAutoConnect = true)
        {
            socketCfg = Config;
            _log = logWrite;
            if (IsAutoConnect)
            {
                Connect();
            }
        }
        /// <summary>
        /// 连接对象
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            try
            {
                IPAddress ipAddress = IPAddress.Parse(socketCfg.Ip);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, socketCfg.Port);
                // Create a TCP/IP socket.  
                socketClient = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.  
                socketClient.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), socketClient);
                connectDone.WaitOne(3000);
                return true;
            }
            catch (Exception ex)
            {
                _log.WriteLog("AsynTcpClient connect error=" + ex.Message);
            }
            return false;
        }
        /// <summary>
        /// 连接回调
        /// </summary>
        /// <param name="AsynRes"></param>
        private void ConnectCallback(IAsyncResult AsynRes)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)AsynRes.AsyncState;
                // Complete the connection.
                client.EndConnect(AsynRes);
                _log.WriteLog(string.Format("Socket connected to {0}", client.RemoteEndPoint.ToString()));
                // Signal that the connection has been made.  
                connectDone.Set();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                _log.WriteLog("ConnectCallback error=" + ex.Message);
            }
        }
        /// <summary>
        /// 发送和接收一次数据
        /// </summary>
        /// <param name="Command"></param>
        /// <param name="IsReceiveData">发送过后是否需要接受数据</param>
        /// <returns></returns>
        public bool SendAndReceiveDataAsync(string Command, bool IsReceiveData = false)
        {
            // Send test data to the remote device. 
            bool bit = false;
            if (socketClient != null && socketClient.Connected)
            {
                try
                {
                    // Convert the string data to byte data using ASCII encoding.  
                    byte[] byteData = Encoding.ASCII.GetBytes(Command);

                    // Begin sending the data to the remote device.  
                    socketClient.BeginSend(byteData, 0, byteData.Length, 0,
                                           new AsyncCallback(SendCallback), socketClient);

                    bit = sendDone.WaitOne(5000);
                    if (bit && IsReceiveData)
                    {
                        // Receive the response from the remote device.  
                        Receive(socketClient);
                        bit = receiveDone.WaitOne(5000);//5S超时
                    }

                    return bit;
                }
                catch (Exception ex)
                {
                    _log.WriteLog("SendAndReceiveDataAsync error=" + ex.Message);
                }
            }
            return false;
        }
        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(ar);
                Console.WriteLine(DateTime.Now + "Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.  
                sendDone.Set();
            }
            catch (Exception ex)
            {
                _log.WriteLog("SendCallback error" , ex);
            }
        }
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="client"></param>
        private void Receive(Socket client)
        {
            try
            {
                // Create the state object.  
                socketReceiveObject = new TcpReceiveObject();
                socketReceiveObject.workSocket = client;

                // Begin receiving the data from the remote device.  
                client.BeginReceive(socketReceiveObject.buffer, 0, TcpReceiveObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), socketReceiveObject);
            }
            catch (Exception ex)
            {
                _log.WriteLog("Receive error", ex);
            }
        }
        /// <summary>
        /// 接收回调
        /// </summary>
        /// <param name="AsynRes"></param>
        private void ReceiveCallback(IAsyncResult AsynRes)
        {
            try
            {
                // Retrieve the state object and the client socket   
                // from the asynchronous state object.  
                TcpReceiveObject state = (TcpReceiveObject)AsynRes.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.  
                int bytesRead = client.EndReceive(AsynRes);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.  
                    state.ReceiveInfo.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                    if (socketCfg.MultiplyReceiveMode)//只是返回了一部分数据，需要再继续等待对方发送其他数据
                    {
                        // Get the rest of the data.  
                        client.BeginReceive(state.buffer, 0, TcpReceiveObject.BufferSize, 0,
                                            new AsyncCallback(ReceiveCallback), state);
                    }
                }

                if (bytesRead == 0 || socketCfg.MultiplyReceiveMode == false)//数据接收完成，或者数据已经一次性接收完毕
                {
                    // All the data has arrived; put it in response.  
                    if (state.ReceiveInfo.Length > 1)
                    {
                        Raise_MessageReceivedEvent(client, state.ReceiveInfo.ToString());
                    }
                    // Signal that all bytes have been received.  
                    receiveDone.Set();
                }
            }
            catch (Exception ex)
            {
                _log.WriteLog("ReceiveCallback error", ex);
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void DisConnect()
        {
            if (socketClient != null && socketClient.Connected)
            {
                try
                {
                    socketClient.Shutdown(SocketShutdown.Both);
                    socketClient.Close();
                }
                catch (Exception ex)
                { }
            }
        }

        public delegate void MessageReceivedDelegate(object sender, string message);
        /// <summary>
        /// 
        /// </summary>
        public event MessageReceivedDelegate MessageReceivedEvent = delegate { };
        /// <summary>
        /// 触发事件
        /// </summary>
        public void Raise_MessageReceivedEvent(object sender, string message)
        {
            if (MessageReceivedEvent != null)
                MessageReceivedEvent(sender, message);
        }
    }
}
