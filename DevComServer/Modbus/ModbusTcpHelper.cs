using HslCommunication;
using HslCommunication.ModBus;
using iFactory.CommonLibrary.Interface;
using System;

namespace iFactory.DevComServer
{
    public class ModbusTcpHelper: IPLCHelper
    {
        private readonly PLCDevice _device;
        private ModbusTcpNet _deviceDriver = null;
        private readonly ILogWrite _log;

        public ModbusTcpHelper(PLCDevice device, ILogWrite logWrite)
        {
            _device = device;
            _log = logWrite;
        }

        public void ConnectToPlc()
        {
            _deviceDriver?.ConnectClose();
            _deviceDriver?.Dispose();
            _deviceDriver = new ModbusTcpNet(_device.Ip, _device.Port, _device.Station);
            _deviceDriver.AddressStartWithZero = true;//首地址从0开始
            _deviceDriver.DataFormat = HslCommunication.Core.DataFormat.ABCD;
            _deviceDriver.IsStringReverse = false; //字符串跌倒
            _deviceDriver.SetPersistentConnection();
            OperateResult res= _deviceDriver.ConnectServer();
            if(res.IsSuccess)
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
        /// 检查连接，并将结果写入ConnectStatus
        /// </summary>
        /// <returns></returns>
        public void CheckConnect()
        {
            if (_deviceDriver != null)
            {
                bool res = false;
                _device.IsConnected = ReadValueDi("0", out res);
            }
            else
            {
                _device.IsConnected = false;
            }
        }
        #region 写值
        public bool WriteValue(string Address, bool value)
        {
            throw new NotImplementedException();
        }

        public bool WriteValue(string Address, short value)
        {
            OperateResult res = _deviceDriver.Write(Address, value);
            if (res.IsSuccess)
            {
                return true;
            }
            else
            {
                _log.WriteLog($"modbus{_device.Ip}写入地址{Address}值{value}失败");
                return false;
            }
        }

        public bool WriteValue(string Address, int value)
        {
            OperateResult res = _deviceDriver.Write(Address, value);
            if (res.IsSuccess)
            {
                return true;
            }
            else
            {
                _log.WriteLog($"modbus{_device.Ip}写入地址{Address}{value}失败");
                return false;
            }
        }

        public bool WriteValue(string Address, float value)
        {
            OperateResult res = _deviceDriver.Write(Address, value);
            if (res.IsSuccess)
            {
                return true;
            }
            else
            {
                _log.WriteLog($"modbus{_device.Ip}写入地址{Address}{value}失败");
                return false;
            }
        }

        public bool WriteValue(string address, string value)
        {
            throw new NotImplementedException();
        }
        public bool BatchWriteValue(string Address, short[] value)
        {
            if (_device.IsConnected)
            {
                OperateResult res = _deviceDriver.Write(Address, value);
                if (res.IsSuccess)
                {
                    return true;
                }
                else
                {
                    _log.WriteLog($"modbus{_device.Ip}写入地址{Address}值{value}失败");
                    return false;
                }
            }
            else
            {
                _log.WriteLog($"modbus{_device.Ip}通信未连接，写入地址{Address}值{value}失败");
            }
            return false;
        }
        #endregion

        #region 读值
        public bool ReadValueDi(string Address, out bool DiData)
        {
            OperateResult<bool> result = _deviceDriver.ReadCoil(Address);
            if (result.IsSuccess)
            {
                DiData = result.Content;
                return true;
            }
            else
            {
                DiData = false;
                return false;
            }
        }
        public bool ReadValue(string Address, out bool value)
        {
            throw new NotImplementedException();
        }

        public bool ReadValue(string Address, out short value)
        {
            try
            {
                OperateResult<short> res = _deviceDriver.ReadInt16(Address);
                if (res.IsSuccess)
                {
                    value = res.Content;
                    return true;
                }
            }
            catch (Exception ex)
            {
                _log.WriteLog($"modbus{_device.Ip}读取地址{Address}失败",ex);
            }
            value = 0;
            return false;
        }

        public bool ReadValue(string Address, out int value)
        {
            try
            {
                OperateResult<int> res = _deviceDriver.ReadInt32(Address);
                if (res.IsSuccess)
                {
                    value = res.Content;
                    return true;
                }
            }
            catch (Exception ex)
            {
                _log.WriteLog($"modbus{_device.Ip}读取地址{Address}失败", ex);
            }
            value = 0;
            return false;
        }

        public bool ReadValue(string Address, out float value)
        {
            try
            {
                OperateResult<float> res = _deviceDriver.ReadFloat(Address);
                if (res.IsSuccess)
                {
                    value = res.Content;
                    return true;
                }
            }
            catch (Exception ex)
            {
                _log.WriteLog($"modbus{_device.Ip}读取地址{Address}失败", ex);
            }
            value = 0;
            return false;
        }
        #endregion

        #region 批量读取
        public bool BatchReadValue(string Address, ushort length, out bool[] value)
        {
            throw new NotImplementedException();
        }

        public bool BatchReadValue(string Address, ushort length, out short[] value)
        {
            OperateResult<short[]> result = _deviceDriver.ReadInt16(Address, length);
            if (result.IsSuccess)
            {
                value = result.Content;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        public bool BatchReadValue(string Address, ushort length, out int[] value)
        {
            OperateResult<int[]> result = _deviceDriver.ReadInt32(Address, length);
            if (result.IsSuccess)
            {
                value = result.Content;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        public bool BatchReadValue(string Address, ushort length, out float[] value)
        {
            OperateResult<float[]> result = _deviceDriver.ReadFloat(Address, length);
            if (result.IsSuccess)
            {
                value = result.Content;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        public bool BatchReadValue(string Address, ushort length, out string[] value)
        {
            throw new NotImplementedException();
        }
        #endregion

        public void Dispose()
        {
            _deviceDriver?.ConnectClose();
            _deviceDriver?.Dispose();
        }

    }
}
