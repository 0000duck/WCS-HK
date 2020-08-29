﻿using HslCommunication;
using HslCommunication.ModBus;
using iFactory.CommonLibrary.Interface;
using System;

namespace iFactory.DevComServer
{
    public class ModbusTcpHelper: IPLCHelper
    {
        private readonly PLCDevice _device;
        private ModbusTcpNet _deviceModbus = null;
        private readonly ILogWrite _log;

        public ModbusTcpHelper(PLCDevice device, ILogWrite logWrite)
        {
            _device = device;
            _log = logWrite;
        }

        public void ConnectToPlc()
        {
            _deviceModbus?.ConnectClose();
            _deviceModbus?.Dispose();
            _deviceModbus = new ModbusTcpNet(_device.Ip, _device.Port, _device.Station);
            _deviceModbus.AddressStartWithZero = true;//首地址从0开始
            _deviceModbus.DataFormat = HslCommunication.Core.DataFormat.ABCD;
            _deviceModbus.IsStringReverse = false; //字符串跌倒
            _deviceModbus.SetPersistentConnection();
            _log.WriteLog($"modbus连接成功{_device.Ip}");
        }
        /// <summary>
        /// 检查连接，并将结果写入ConnectStatus
        /// </summary>
        /// <returns></returns>
        public void CheckConnect()
        {
            if (_deviceModbus != null)
            {
                bool res = false;
                _device.IsConnected = ReadValueDi("0", out res);
            }
            else
            {
                _device.IsConnected = false;
            }
        }
        public bool WriteValue(string Address, bool value)
        {
            throw new NotImplementedException();
        }

        public bool WriteValue(string Address, short value)
        {
            OperateResult res = _deviceModbus.Write(Address, value);
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

        public bool WriteValue(string Address, int value)
        {
            OperateResult res = _deviceModbus.Write(Address, value);
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
            OperateResult res = _deviceModbus.Write(Address, value);
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
        public bool ReadValueDi(string Address, out bool DiData)
        {
            OperateResult<bool> result = _deviceModbus.ReadCoil(Address);
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
                OperateResult<short> res = _deviceModbus.ReadInt16(Address);
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
                OperateResult<int> res = _deviceModbus.ReadInt32(Address);
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
                OperateResult<float> res = _deviceModbus.ReadFloat(Address);
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

        public bool BatchReadValue(string Address, ushort length, out bool[] value)
        {
            throw new NotImplementedException();
        }

        public bool BatchReadValue(string Address, ushort length, out short[] value)
        {
            OperateResult<short[]> result = _deviceModbus.ReadInt16(Address, length);
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
            OperateResult<int[]> result = _deviceModbus.ReadInt32(Address, length);
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
            OperateResult<float[]> result = _deviceModbus.ReadFloat(Address, length);
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

        public bool BatchWriteValue(string Address, short[] value)
        {
            OperateResult res = _deviceModbus.Write(Address, value);
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

        public void Dispose()
        {
            _deviceModbus?.ConnectClose();
            _deviceModbus?.Dispose();
        }

        
    }
}
