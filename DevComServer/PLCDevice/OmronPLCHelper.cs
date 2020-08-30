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
        public OmronFinsNet deviceDriver { set; get; }
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
            deviceDriver = new OmronFinsNet(_device.Ip, _device.Port)
            {
                ConnectTimeOut = 2000
            };
            ipArray = _device.Ip.Split('.');
            values = System.BitConverter.GetBytes(int.Parse(ipArray[ipArray.Length - 1]));
            deviceDriver.SA1 = values[0]; // PC网络号，PC的IP地址的最后一个数.假如你的电脑的Ip地址为192.168.0.13，那么这个值就是13
            ipArray = _device.Ip.Split('.');
            values = System.BitConverter.GetBytes(int.Parse(ipArray[ipArray.Length - 1]));
            deviceDriver.DA1 = values[0]; // 0x10 PLC网络号，PLC的IP地址的最后一个数.假如你的PLC的Ip地址为192.168.0.10，那么这个值就是10
            //omronTcpNet.SA1 = 0x12; // PC网络号，PC的IP地址的最后一个数
            //omronTcpNet.DA1 = 0x10; // PLC网络号，PLC的IP地址的最后一个数
            deviceDriver.DA2 = 0x00; // PLC单元号，通常为0
            deviceDriver.ConnectServer();
            deviceDriver.SetPersistentConnection();   // 设置长连接
            OperateResult res = deviceDriver.ConnectServer();
            if (res.IsSuccess)
            {
                _log.WriteLog($"{_device.Name}{_device.Ip}连接成功");
            }
            else
            {
                _log.WriteLog($"{_device.Name}{_device.Ip}连接失败");
            }
            _device.IsConnected = res.IsSuccess;
        }
        /// <summary>
        /// 检查是否连接成功,通过读取PLC的M0.0固定地址来判断
        /// </summary>
        /// <returns></returns>
        public void CheckConnect()
        {
            try
            {
                if (deviceDriver != null)
                {
                    OperateResult<short> res = deviceDriver.ReadInt16("D100");
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
                if (_device.IsConnected)
                {
                    OperateResult res = deviceDriver.Write(Address, value);
                    if (res.IsSuccess == false)
                    {
                        _log.WriteLog($"{_device.Name}{_device.Ip}，写入地址{Address}值{value}失败");
                    }
                    return res.IsSuccess;
                }
                else
                {
                    _log.WriteLog($"{_device.Name}{_device.Ip}通信未连接，写入地址{Address}值{value}失败");
                }
                return false;
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
                if (_device.IsConnected)
                {
                    OperateResult res = deviceDriver.Write(Address, value);
                    if (res.IsSuccess == false)
                    {
                        _log.WriteLog($"{_device.Name}{_device.Ip}，写入地址{Address}值{value}失败");
                    }
                    return res.IsSuccess;
                }
                else
                {
                    _log.WriteLog($"{_device.Name}{_device.Ip}通信未连接，写入地址{Address}值{value}失败");
                }
                return false;
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
                if (_device.IsConnected)
                {
                    OperateResult res = deviceDriver.Write(Address, value);
                    if (res.IsSuccess == false)
                    {
                        _log.WriteLog($"{_device.Name}{_device.Ip}，写入地址{Address}值{value}失败");
                    }
                    return res.IsSuccess;
                }
                else
                {
                    _log.WriteLog($"{_device.Name}{_device.Ip}通信未连接，写入地址{Address}值{value}失败");
                }
                return false;
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
                if (_device.IsConnected)
                {
                    OperateResult res = deviceDriver.Write(Address, value);
                    if (res.IsSuccess == false)
                    {
                        _log.WriteLog($"{_device.Name}{_device.Ip}，写入地址{Address}值{value}失败");
                    }
                    return res.IsSuccess;
                }
                else
                {
                    _log.WriteLog($"{_device.Name}{_device.Ip}通信未连接，写入地址{Address}值{value}失败");
                }
                return false;
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
                OperateResult<bool> res = deviceDriver.ReadBool(Address);
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
                OperateResult<short> res = deviceDriver.ReadInt16(Address);
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
                OperateResult<short[]> res = deviceDriver.ReadInt16(Address,length);
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
            if(deviceDriver !=null)
            {
                deviceDriver.Dispose();
            }
        }

        

        public bool BatchWriteValue(string Address, short[] value)
        {
            throw new NotImplementedException();
        }

        public bool WriteValue(string address, string value)
        {
            throw new NotImplementedException();
        }
    }
}
