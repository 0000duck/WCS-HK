using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using iFactory.CommonLibrary.Interface;

namespace iFactory.DevComServer
{
    public class TcpServer : IDisposable,IPLCHelper
    {
        private readonly PLCDevice _device;
        private TcpListener tcpListener;
        private Task listenTask;
        private readonly IPAddress serverAddress;
        private readonly int serverPort;
        private CancellationTokenSource tokenSource;
        private CancellationToken token;
        private Dictionary<string, TcpClient> ClientDic = new Dictionary<string, TcpClient>();
        private readonly ILogWrite _log;

        public TcpServer(string ip, int port, ILogWrite logWrite)
        {
            _log = logWrite;
            serverAddress = IPAddress.Parse(ip);
            serverPort = port;
        }
        public TcpServer(PLCDevice pLCDevice, ILogWrite logWrite)
        {
            _log = logWrite;
            _device = pLCDevice;
            serverAddress = IPAddress.Parse(_device.Ip);
            serverPort = _device.Port;
        }
        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            if (tcpListener != null)
            {
                tcpListener.Stop();
                tcpListener = null;
            }

            this.tcpListener = new TcpListener(serverAddress, serverPort);
            tcpListener.Start();
            tokenSource = new CancellationTokenSource();
            token = tokenSource.Token;
            if (this.listenTask == null || this.listenTask.Status != TaskStatus.Running)
            {
                this.listenTask = Task.Factory.StartNew(() => ListenForClients(), token);//TaskCreationOptions.LongRunning,
            }
           
        }
        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            if (tcpListener != null)
            {
                tcpListener.Stop();
            }
            tcpListener = null;
            tokenSource.Cancel();
        }
        private void ListenForClients()
        {
            try
            {
                while (true)
                {
                    _log.WriteLog("等待新的客户端连接...");
                    TcpClient client = tcpListener.AcceptTcpClient();
                    ClientDic.Add(client.Client.RemoteEndPoint.ToString(), client);
                    _log.WriteLog($"客户端{client.Client.RemoteEndPoint}已连接!");
                    _device.IsConnected = true;
                    Thread t = new Thread(new ParameterizedThreadStart(HandleDeivceRead));
                    t.Start(client);
                }
            }
            catch (Exception ex)
            {
                _log.WriteLog($"ListenForClients发生错误{ex.Message}");
            }
        }
        /// <summary>
        /// 处理读取
        /// </summary>
        /// <param name="tcpClientObj"></param>
        public void HandleDeivceRead(Object tcpClientObj)
        {
            TcpClient client = (TcpClient)tcpClientObj;
            var stream = client.GetStream();

            try
            {
                while (!tokenSource.IsCancellationRequested)
                {
                    if (stream.CanRead)
                    {
                        byte[] myReadBuffer = new byte[1000];//限定读取32个
                        string myCompleteMessage = string.Empty;
                        int numberOfBytesRead = 0;

                        do
                        {
                            numberOfBytesRead = stream.Read(myReadBuffer, 0, myReadBuffer.Length);
                            myCompleteMessage += BitConverter.ToString(myReadBuffer, 0, numberOfBytesRead);//使用原始数据
                            myCompleteMessage = myCompleteMessage.Replace('-', ' ');
                        }
                        while (stream.DataAvailable);
                        _log.WriteLog($"{client.Client.RemoteEndPoint}: 读取到数据: {myCompleteMessage}");
                        if (numberOfBytesRead == 10)//心跳包
                        {

                        }
                        else if (numberOfBytesRead == 32)//数据返回
                        {

                        }
                    }
                    else
                    {
                        Console.WriteLine("当前不可读取数据");
                    }
                    Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                _log.WriteLog($"HandleDeivce Exception", e);
                if (client != null)
                {
                    client.Close();
                }
            }
        }
        /// <summary>
        /// 发送消息到客户端
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="client"></param>
        public void SendMessageToClient(string Message,TcpClient client=null)
        {
            try
            {
                if(client==null && ClientDic.Count>0)
                {
                    client = ClientDic.FirstOrDefault().Value;
                }
                var stream = client.GetStream();
                if (stream != null && stream.CanWrite)
                {
                    Byte[] reply = System.Text.Encoding.ASCII.GetBytes(Message);
                    stream.Write(reply, 0, reply.Length);
                    stream.Flush();
                    _log.WriteLog($"{client.Client.RemoteEndPoint}: 发送消息: {Message}");
                }
            }
            catch (Exception ex)
            {
                _log.WriteLog($"{client.Client.RemoteEndPoint}: 发送消息错误: {ex.Message}");
            }
        }
        public void Dispose()
        {
            Stop();
        }

        public void ConnectToPlc()
        {
            Start();
        }

        public void CheckConnect()
        {
            
        }

        public bool WriteValue(string Address, bool value)
        {
            throw new NotImplementedException();
        }

        public bool WriteValue(string Address, short value)
        {
            throw new NotImplementedException();
        }

        public bool WriteValue(string Address, int value)
        {
            throw new NotImplementedException();
        }

        public bool WriteValue(string Address, float value)
        {
            throw new NotImplementedException();
        }

        public bool WriteValue(string address, string value)
        {
            throw new NotImplementedException();
        }

        public bool ReadValue(string Address, out bool value)
        {
            throw new NotImplementedException();
        }

        public bool ReadValue(string Address, out short value)
        {
            throw new NotImplementedException();
        }

        public bool ReadValue(string Address, out int value)
        {
            throw new NotImplementedException();
        }

        public bool ReadValue(string Address, out float value)
        {
            throw new NotImplementedException();
        }

        public bool BatchReadValue(string Address, ushort length, out bool[] value)
        {
            throw new NotImplementedException();
        }

        public bool BatchReadValue(string Address, ushort length, out short[] value)
        {
            throw new NotImplementedException();
        }

        public bool BatchReadValue(string Address, ushort length, out int[] value)
        {
            throw new NotImplementedException();
        }

        public bool BatchReadValue(string Address, ushort length, out float[] value)
        {
            throw new NotImplementedException();
        }

        public bool BatchReadValue(string Address, ushort length, out string[] value)
        {
            throw new NotImplementedException();
        }

        public bool BatchWriteValue(string Address, short[] value)
        {
            throw new NotImplementedException();
        }
    }
}
