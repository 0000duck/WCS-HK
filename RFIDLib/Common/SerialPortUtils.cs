using System;
using System.Collections.Generic;

using System.Text;
using System.IO.Ports;

namespace RFIDLib
{
    public class SerialPortUtils
    {
        string[] portNameList;
        
        public string[] getPortNameList()
        {
            this.portNameList = SerialPort.GetPortNames();
            return portNameList;
        }
        /// <summary>
        /// 打开串口
        /// </summary>
        /// <param name="portName"></param>
        /// <returns></returns>
        public SerialPort OpenPort(string portName, LogHandleUtils logHandleUtils)
        {
            SerialPort sp = new SerialPort();
            sp.PortName = portName;
            sp.BaudRate = 115200;
            sp.DataBits = 8;
            sp.DtrEnable = true;
            try
            {
                if (!sp.IsOpen)
                {
                    sp.Open();
                }
            }
            catch (Exception ex)
            {
                logHandleUtils.writeLog("串口打开出错:"+ex.Message);
            }
            return sp;
        }
        /// <summary>
        /// 关闭串口
        /// </summary>
        /// <param name="portName"></param>
        /// <returns></returns>
        public void ClosePort(SerialPort sp)
        {
            try
            {
                if (sp.IsOpen)
                {
                    sp.Close();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }

}