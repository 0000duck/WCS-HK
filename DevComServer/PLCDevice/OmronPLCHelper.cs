using System;
using HslCommunication;
using HslCommunication.Profinet.Omron;
using iFactory.CommonLibrary.Interface;

namespace iFactory.DevComServer
{
    public class OmronPLCHelper:  IPLCHelper
    {
        private readonly PLCDevice _device;
        /// <summary>
        /// 操作对象
        /// </summary>
        public OmronFinsNet omronTcpNet { set; get; }
        private readonly ILogWrite _log;

        public OmronPLCHelper(PLCDevice device,ILogWrite logWrite)
        {
            _device = device;
            _log = logWrite;
        }
        public void ConnectToPlc()
        {
            string[] ipArray;
            byte[] values;
            omronTcpNet = new OmronFinsNet(_device.Ip, _device.Port)
            {
                ConnectTimeOut = 2000
            };
            ipArray = _device.Ip.Split('.');
            values = System.BitConverter.GetBytes(int.Parse(ipArray[ipArray.Length - 1]));
            omronTcpNet.SA1 = values[0]; // PC网络号，PC的IP地址的最后一个数.假如你的电脑的Ip地址为192.168.0.13，那么这个值就是13
            ipArray = _device.Ip.Split('.');
            values = System.BitConverter.GetBytes(int.Parse(ipArray[ipArray.Length - 1]));
            omronTcpNet.DA1 = values[0]; // 0x10 PLC网络号，PLC的IP地址的最后一个数.假如你的PLC的Ip地址为192.168.0.10，那么这个值就是10
            //omronTcpNet.SA1 = 0x12; // PC网络号，PC的IP地址的最后一个数
            //omronTcpNet.DA1 = 0x10; // PLC网络号，PLC的IP地址的最后一个数
            omronTcpNet.DA2 = 0x00; // PLC单元号，通常为0
            omronTcpNet.ConnectServer();
            omronTcpNet.SetPersistentConnection();   // 设置长连接   
        }
        public void ConnectToPlc(string Address)
        {
            throw new NotImplementedException();
        }

        public void ConnectToPlc(string Address, int Port, PLCType plcType)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// 检查是否连接成功,通过读取PLC的M0.0固定地址来判断
        /// </summary>
        /// <returns></returns>
        public void CheckConnect()
        {
            try
            {
                if (omronTcpNet != null)
                {
                    OperateResult<short> res = omronTcpNet.ReadInt16("D100");
                    _device.IsConnected = res.IsSuccess;
                }
                else
                {
                    _device.IsConnected = false;
                }

            }
            catch (Exception ex)
            {
                _log.WriteLog(ex.Message);
                _device.IsConnected = false;
            }
        }

        #region 写入值
        /// <summary>
        /// 向PLC写文本值
        /// </summary>
        /// <param name="Addr">PLC地址</param>
        /// <param name="value">写入值</param>
        /// <returns></returns>
        public bool WriteValue(string Address, bool value)
        {
            try
            {
                OperateResult res = omronTcpNet.Write(Address, value);
                return res.IsSuccess;
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
        public bool WriteValue(string Address, short value)
        {
            try
            {
                OperateResult res = omronTcpNet.Write(Address, value);
                return res.IsSuccess;
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
        public bool WriteValue(string Address, int value)
        {
            try
            {
                OperateResult res = omronTcpNet.Write(Address, value);
                return res.IsSuccess;
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
        public bool WriteValue(string Address, float value)
        {
            try
            {
                OperateResult res = omronTcpNet.Write(Address, value);
                return res.IsSuccess;
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

        

        public bool BatchWriteValue(string Address, short[] value)
        {
            throw new NotImplementedException();
        }

       
    }
}
