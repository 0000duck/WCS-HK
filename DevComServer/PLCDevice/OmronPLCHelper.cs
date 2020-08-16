using System;
using HslCommunication;
using HslCommunication.Profinet.Omron;
using iFactory.CommonLibrary;
using iFactory.CommonLibrary.Interface;

namespace iFactory.DevComServer
{
    public class OmronPLCHelper: BasePLCHelper, IPLCHelper
    {
        public string PLCLocalPcAddr { set; get; } = "192.168.100.12";
        private object obj = new object();
        /// <summary>
        /// 操作对象
        /// </summary>
        public OmronFinsNet omronTcpNet { set; get; }
        private readonly ILogWrite _log;

        public OmronPLCHelper(ILogWrite logWrite)
        {
            _log = logWrite;
        }
        /// <summary>
        /// 连接PLC
        /// </summary>
        public void ConnectToPlc(string Addr = null, int Port = 0)
        {
            string[] ipArray;
            byte[] values;
            try
            {
                if(!string.IsNullOrEmpty(Addr))
                {
                    PLCAddr = Addr;
                }
                if(Port>0)
                {
                    PLCPort = Port;
                }
                omronTcpNet = new OmronFinsNet(PLCAddr, PLCPort)
                {
                    ConnectTimeOut = 2000
                };
                ipArray = PLCLocalPcAddr.Split('.');
                values = System.BitConverter.GetBytes(int.Parse(ipArray[ipArray.Length - 1]));
                omronTcpNet.SA1 = values[0]; // PC网络号，PC的IP地址的最后一个数.假如你的电脑的Ip地址为192.168.0.13，那么这个值就是13
                ipArray = PLCAddr.Split('.');
                values = System.BitConverter.GetBytes(int.Parse(ipArray[ipArray.Length - 1]));
                omronTcpNet.DA1 = values[0]; // 0x10 PLC网络号，PLC的IP地址的最后一个数.假如你的PLC的Ip地址为192.168.0.10，那么这个值就是10
                //omronTcpNet.SA1 = 0x12; // PC网络号，PC的IP地址的最后一个数
                //omronTcpNet.DA1 = 0x10; // PLC网络号，PLC的IP地址的最后一个数
                omronTcpNet.DA2 = 0x00; // PLC单元号，通常为0
                                        //siemensTcpNet.LogNet = LogNet;
                omronTcpNet.ConnectServer();
                omronTcpNet.SetPersistentConnection();   // 设置长连接              
            }
            catch (Exception ex)
            {
                _log.WriteLog(ex.Message);
            }
        }
        /// <summary>
        /// 检查是否连接成功,通过读取PLC的M0.0固定地址来判断
        /// </summary>
        /// <returns></returns>
        public bool CheckConnect()
        {
            try
            {
                //OperateResult<bool> res = omronTcpNet.ReadBool("20.0");
                OperateResult<short> res = omronTcpNet.ReadInt16("D100");

                return res.IsSuccess;
            }
            catch (Exception ex)
            {
                _log.WriteLog(ex.Message);
            }
            return false;
        }

        #region 写入值
        /// <summary>
        /// 向PLC写文本值
        /// </summary>
        /// <param name="Addr">PLC地址</param>
        /// <param name="value">写入值</param>
        /// <returns></returns>
        public bool WriteValue(string address, bool value)
        {
            try
            {
                lock (obj)
                {
                    OperateResult res = omronTcpNet.Write(address, value);
                    return res.IsSuccess;
                }
            }
            catch (Exception ex)
            {
                _log.WriteLog(ex.Message);
            }
            return false;
        }
        /// <summary>
        /// 向PLC写整型值
        /// </summary>
        /// <param name="Addr">PLC地址</param>
        /// <param name="value">写入值</param>
        /// <returns></returns>
        public bool WriteValue(string address, short value)
        {
            try
            {
                lock (obj)
                {
                    OperateResult res = omronTcpNet.Write(address, value);
                    return res.IsSuccess;
                }
            }
            catch (Exception ex)
            {
                _log.WriteLog(ex.Message);
            }
            return false;
        }
        /// <summary>
        /// 向PLC写整型值int  PLC为Dint类型
        /// </summary>
        /// <param name="Addr">PLC地址</param>
        /// <param name="value">写入值</param>
        /// <returns></returns>
        public bool WriteValue(string address, int value)
        {
            try
            {
                lock (obj)
                {
                    OperateResult res = omronTcpNet.Write(address, value);
                    return res.IsSuccess;
                }
            }
            catch (Exception ex)
            {
                _log.WriteLog(ex.Message);
            }
            return false;
        }
        /// <summary>
        /// 向PLC写浮点型
        /// </summary>
        /// <param name="Addr">PLC地址</param>
        /// <param name="value">写入值</param>
        /// <returns></returns>
        public bool WriteValue(string address, float value)
        {
            try
            {
                lock (obj)
                {
                    OperateResult res = omronTcpNet.Write(address, value);
                    return res.IsSuccess;
                }
            }
            catch (Exception ex)
            {
                _log.WriteLog(ex.Message);
            }
            return false;
        }
        #endregion

        #region bool读取
        /// <summary>
        /// 读取标识位
        /// </summary>
        /// <param name="Address">PLC地址</param>
        /// <param name="value">读取的结果</param>
        /// <returns></returns>
        public bool ReadValue(string Address, out bool value)
        {
            try
            {
                OperateResult<bool> res = omronTcpNet.ReadBool(Address);
                if (res.IsSuccess)
                {
                    value = res.Content;
                    return true;
                }
            }
            catch (Exception ex)
            {
                _log.WriteLog(ex.Message);
            }
            value = false;
            return false;
        }
        public bool BatchReadValue(string Address, ushort length, out bool[] value)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region short读取
        public bool ReadValue(string Address, out short value)
        {
            try
            {
                OperateResult<short> res = omronTcpNet.ReadInt16(Address);
                if (res.IsSuccess)
                {
                    value = res.Content;
                    return true;
                }
            }
            catch (Exception ex)
            {
                _log.WriteLog(ex.Message);
            }
            value = -1;
            return false;
        }
        public bool BatchReadValue(string Address, ushort length, out short[] value)
        {
            try
            {
                OperateResult<short[]> res = omronTcpNet.ReadInt16(Address,length);
                if (res.IsSuccess)
                {
                    value = res.Content;
                    return true;
                }
            }
            catch (Exception ex)
            {
                _log.WriteLog(ex.Message);
            }
            value = null;
            return false;
        }
        #endregion
        #region int读取
        public bool ReadValue(string Address, out int value)
        {
            throw new NotImplementedException();
        }
        public bool BatchReadValue(string Address, ushort length, out int[] value)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region float读取
        public bool ReadValue(string Address, out float value)
        {
            throw new NotImplementedException();
        }
        public bool BatchReadValue(string Address, ushort length, out float[] value)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region string读取
        public bool BatchReadValue(string Address, ushort length, out string[] value)
        {
            throw new NotImplementedException();
        }
        #endregion

        public void Dispose()
        {
            if(omronTcpNet !=null)
            {
                omronTcpNet.Dispose();
            }
        }

        public void ConnectToPlc(string Addr)
        {
            throw new NotImplementedException();
        }

        public void ConnectToPlc(string Addr, int Port, PLCType plcType)
        {
            throw new NotImplementedException();
        }

        public void ReConnectToPlc()
        {
            throw new NotImplementedException();
        }
    }
}
