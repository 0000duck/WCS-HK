using iFactory.CommonLibrary.Interface;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

namespace iFactory.DevComServer
{
    /// <summary>
    /// 串口通信
    /// </summary>
    public class SerialHelper: SerialComBase, ISerialHelper
    {
        private SerialPort serialPort;
        private SerialConfig serialCfg;
        private Thread cycTask;
        private readonly ILogWrite _log;

        public SerialHelper(SerialConfig serialConfig, ILogWrite logWrite)
        {
            serialCfg = serialConfig;
            _log = logWrite;
            SerialPortInitial();
        }
        /// <summary>
        /// 串口初始化
        /// </summary>
        public void SerialPortInitial()
        {
            try
            {
                serialPort = new SerialPort();

                serialPort.PortName = serialCfg.PortName;//串口号
                serialPort.BaudRate = serialCfg.PortBaudRate;//波特率
                serialPort.DataBits = serialCfg.PortDataBits;//数据位
                serialPort.StopBits = serialCfg.PortStopBits;
                serialPort.Parity = serialCfg.PortParity;

                //准备就绪              
                serialPort.DtrEnable = true;
                serialPort.RtsEnable = true;
                serialPort.ReadTimeout = 5000;//设置数据读取超时为5秒
                serialPort.Open();
               
                if (serialCfg.ReadCommand.Length>0)//存在周期读取命令
                {
                    cycTask = new Thread(CycSendCommand);
                    cycTask.IsBackground = true;
                    cycTask.Start();
                }
                serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
            }
            catch(Exception ex)
            {
                _log.WriteLog("串口打开出错",ex);
            }
        }
        /// <summary>
        /// 周期命令读取
        /// </summary>
        private void CycSendCommand()
        {
            while(true)
            {
                SendReadCommand(serialCfg.ReadCommand);
                Thread.Sleep(500);
            }
        }
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string receiveBuff = string.Empty;
            byte[] readBuffer;
            int count=0;
            string endString = string.Empty;
            Thread.Sleep(500);
            if (serialPort.IsOpen)
            {
                if (serialCfg.DataFormat == SerialDataFormatEnum.Ascii) //接收格式为ASCII
                {
                    try
                    {
                        //receiveMsg = serialPort.ReadLine().Trim();//read line需要line符号
                        //byte[] readBuffer = new byte[serialPort.ReadBufferSize + 1];
                        readBuffer = new byte[serialPort.BytesToRead];
                        count = serialPort.Read(readBuffer, 0, serialPort.BytesToRead);        //读取串口数据(监听)
                        if (count >1)
                        {
                            receiveBuff = Encoding.ASCII.GetString(readBuffer).Trim();
                            Console.WriteLine(DateTime.Now.ToShortTimeString()+" 串口数据接收："+ receiveBuff);
                            endString = receiveBuff.Substring(receiveBuff.Length - 1);
                            if (endString== "\u0003")
                            {
                                receiveBuff = receiveBuff.Substring(0, receiveBuff.Length - 1);//减去1个转义字符
                                Console.WriteLine(DateTime.Now.ToShortTimeString() + " 去除结束字符：" + receiveBuff);
                            }
                            //serialPort.DiscardOutBuffer();
                            double value = 0;
                            double.TryParse(receiveBuff, out value);
                            decimal weight = (decimal)Math.Round(value * (double)serialCfg.Ratio, serialCfg.DecimalNum);//显示格式转换
                            receiveBuff = string.Empty;
                            Raise_MessageReceivedEvent(this, weight);
                            //else
                            //{
                            //    receiveBuff += tmp;
                            //}
                        }
                        else
                        {
                            Console.WriteLine(DateTime.Now.ToShortTimeString() + " 串口数据接收低于1个字符：" + readBuffer.ToString());
                        }
                        
                    }
                    catch (System.Exception ex)
                    {
                        _log.WriteLog("接收出错",ex);
                        return;
                    }
                }
                else //接收格式为HEX
                {
                    try
                    {
                        string input = serialPort.ReadLine();
                        char[] values = input.ToCharArray();
                        foreach (char letter in values)
                        {
                            // Get the integral value of the character.
                            int value = Convert.ToInt32(letter);
                            // Convert the decimal value to a hexadecimal value in string form.
                            //receiveMsg = String.Format("{0:X}", value);
                            //Raise_MessageReceivedEvent(this, receiveMsg);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        _log.WriteLog("接收出错", ex);
                        return;
                    }
                }
                serialPort.DiscardInBuffer(); //清空SerialPort控件的Buffer 
            }
            else
            {
                _log.WriteLog("串口未在打开状态，接收失败");
            }
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public bool SendReadCommand(string command)
        {
            if (!serialPort.IsOpen)
            {
                return false;
            }
            try
            {
                if (serialCfg.DataFormat == SerialDataFormatEnum.Ascii)//以字符串 ASCII 发送
                {
                    serialPort.WriteLine(command);//发送一行数据 
                }
                else
                {
                    //16进制数据格式 HXE 发送
                    char[] values = command.ToCharArray();
                    foreach (char letter in values)
                    {
                        // Get the integral value of the character.
                        int value = Convert.ToInt32(letter);
                        // Convert the decimal value to a hexadecimal value in string form.
                        string hexIutput = String.Format("{0:X}", value);
                        serialPort.WriteLine(hexIutput);
                    }
                }
                return true;
            }
            catch(Exception ex)
            {
                _log.WriteLog("发送命令出错",ex);
            }
            return false;
        }

        public void Dispose()
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }

        /// <summary>
        /// 是否打开
        /// </summary>
        public bool IsOpend
        {
            get
            {
                if(serialPort !=null && serialPort.IsOpen)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
    /// <summary>
    /// 通信基类
    /// </summary>
    public class SerialComBase
    {
        public delegate void MessageReceivedDelegate(object sender, decimal number);
        /// <summary>
        /// 
        /// </summary>
        public event MessageReceivedDelegate MessageReceivedEvent = delegate { };
        /// <summary>
        /// 触发事件
        /// </summary>
        public void Raise_MessageReceivedEvent(object sender, decimal number)
        {
            MessageReceivedEvent?.Invoke(sender, number);
        }
    }
    /// <summary>
    /// 串口标准定义接口
    /// </summary>
    public interface ISerialHelper:IDisposable
    {
        void DataReceived(object sender, SerialDataReceivedEventArgs e);
        bool SendReadCommand(string command);
        void SerialPortInitial();
        bool IsOpend { get; }
    }
    /// <summary>
    /// 串口参数
    /// </summary>
    public class SerialConfig
    {
        /// <summary>
        /// 串口号码
        /// </summary>
        public string PortName { set; get; }
        /// <summary>
        /// 波特率
        /// </summary>
        public int PortBaudRate { set; get; }
        /// <summary>
        /// 数据位
        /// </summary>
        public int PortDataBits { set; get; }
        /// <summary>
        /// 奇偶校验位
        /// </summary>
        public Parity PortParity { set; get; }
        /// <summary>
        /// 停止位
        /// </summary>
        public StopBits PortStopBits { set; get; }
        /// <summary>
        /// 数据格式
        /// </summary>
        public SerialDataFormatEnum DataFormat { set; get; }
        /// <summary>
        /// 周期读取命令
        /// </summary>
        public string ReadCommand { set; get; }
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
    /// 串口数据交换格式
    /// </summary>
    public enum SerialDataFormatEnum
    {
        /// <summary>
        /// ACSII编码
        /// </summary>
        Ascii=0,
        /// <summary>
        /// 十六进制
        /// </summary>
        Hex=1
    }
}
