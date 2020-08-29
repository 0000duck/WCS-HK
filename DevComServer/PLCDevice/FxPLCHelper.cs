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
        public MelsecMcNet fxTcpNet { set; get; }
       
        private readonly ILogWrite _log;

        public FxPLCHelper(PLCDevice device, ILogWrite logWrite)
        {
            _device = device;
            _log = logWrite;
        }
        public void ConnectToPlc()
        {
            fxTcpNet = new MelsecMcNet(_device.Ip, _device.Port);
            fxTcpNet.ConnectTimeOut = 2000; // 网络连接的超时时间
            fxTcpNet.NetworkNumber = 0x00;  // 网络号
            fxTcpNet.NetworkStationNumber = 0x00; // 网络站号    
            fxTcpNet.SetPersistentConnection();
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
                if (fxTcpNet != null)
                {
                    OperateResult<short> res = fxTcpNet.ReadInt16("D100");
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
                OperateResult res = fxTcpNet.Write(Address, value);
                return res.IsSuccess;

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
                OperateResult res = fxTcpNet.Write(Address, value);
                return res.IsSuccess;
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
                OperateResult res = fxTcpNet.Write(Address, value);
                return res.IsSuccess;
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
                OperateResult res = fxTcpNet.Write(Address, value);
                return res.IsSuccess;
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
                OperateResult<bool> res = fxTcpNet.ReadBool(Address);
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
                OperateResult<short> res = fxTcpNet.ReadInt16(Address);
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
                OperateResult<int> res = fxTcpNet.ReadInt32(Address);
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
                OperateResult<float> res = fxTcpNet.ReadFloat(Address);
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
                OperateResult<bool[]> res = fxTcpNet.ReadBool(Address, length);
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
                OperateResult<short[]> res = fxTcpNet.ReadInt16(Address, length);
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
                OperateResult<int[]> res = fxTcpNet.ReadInt32(Address, length);
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
            if(fxTcpNet !=null)
            {
                fxTcpNet.Dispose();
            }
        }

        public bool BatchWriteValue(string Address, short[] value)
        {
            throw new NotImplementedException();
        }
    }
}
