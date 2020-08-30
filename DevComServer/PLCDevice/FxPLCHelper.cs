using System;
using HslCommunication;
using HslCommunication.Profinet.Melsec;
using iFactory.CommonLibrary.Interface;

namespace iFactory.DevComServer
{
    public class FxPLCHelper: IPLCHelper
    {
        private readonly PLCDevice _device;
        /// <summary>
        /// 操作对象.Fx3U使用MelsecA1ENet
        /// </summary>
        public MelsecMcNet deviceDriver { set; get; }
       
        private readonly ILogWrite _log;

        public FxPLCHelper(PLCDevice device, ILogWrite logWrite)
        {
            _device = device;
            _log = logWrite;
        }
        public void ConnectToPlc()
        {
            deviceDriver = new MelsecMcNet(_device.Ip, _device.Port);
            deviceDriver.ConnectTimeOut = 2000; // 网络连接的超时时间
            deviceDriver.NetworkNumber = 0x00;  // 网络号
            deviceDriver.NetworkStationNumber = 0x00; // 网络站号    
            deviceDriver.SetPersistentConnection();
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
        /// 心跳位写入，M100.0
        /// </summary>
        public void WriteHeartBit()
        {
            //WriteValue("M100.0", true);
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

        #region 写值
        public bool WriteValue(string Address, bool value)
        {
            try
            {
                if(_device.IsConnected)
                {
                    OperateResult res = deviceDriver.Write(Address, value);
                    if(res.IsSuccess==false)
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

        #region 读值
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
            value = 0;
            return false;
        }

        public bool ReadValue(string Address, out int value)
        {
            try
            {
                OperateResult<int> res = deviceDriver.ReadInt32(Address);
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
            value = 0;
            return false;
        }

        public bool ReadValue(string Address, out float value)
        {
            try
            {
                OperateResult<float> res = deviceDriver.ReadFloat(Address);
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
            value = 0;
            return false;
        }
        #endregion
        #region 批量读值
        public bool BatchReadValue(string Address, ushort length, out bool[] value)
        {
            try
            {
                OperateResult<bool[]> res = deviceDriver.ReadBool(Address, length);
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

        public bool BatchReadValue(string Address, ushort length, out short[] value)
        {
            try
            {
                OperateResult<short[]> res = deviceDriver.ReadInt16(Address, length);
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

        public bool BatchReadValue(string Address, ushort length, out int[] value)
        {
            try
            {
                OperateResult<int[]> res = deviceDriver.ReadInt32(Address, length);
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

        public bool BatchReadValue(string Address, ushort length, out float[] value)
        {
            throw new NotImplementedException();
        }

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
