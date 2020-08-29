using HslCommunication;
using HslCommunication.Profinet.Siemens;
using iFactory.CommonLibrary.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace iFactory.DevComServer
{
    public class SimensPLCHelper: IPLCHelper
    {
        private readonly PLCDevice _device;
        private SiemensPLCS siemensPLCS= SiemensPLCS.S1200;
        /// <summary>
        /// 操作对象
        /// </summary>
        public SiemensS7Net siemensTcpNet { set; get; }
       
        private readonly ILogWrite _log;

        public SimensPLCHelper(PLCDevice device,ILogWrite logWrite)
        {
            _device = device;
            _log = logWrite;
        }
       
        public void ConnectToPlc()
        {
            siemensPLCS = SiemensPLCS.S1200;
            switch (_device.Type)
            {
                case PLCType.Simens1200:
                    siemensPLCS = SiemensPLCS.S1200;
                    break;
                case PLCType.Simens1500:
                    siemensPLCS = SiemensPLCS.S1500;
                    break;
                case PLCType.Simens300:
                    siemensPLCS = SiemensPLCS.S300;
                    break;
                case PLCType.Simens200Smart:
                    siemensPLCS = SiemensPLCS.S200Smart;
                    break;
            }
            siemensTcpNet?.ConnectClose();
            siemensTcpNet?.Dispose();
            siemensTcpNet = new SiemensS7Net(siemensPLCS, _device.Ip)
            {
                ConnectTimeOut = 2000
            };

            //siemensTcpNet.LogNet = LogNet;
            siemensTcpNet.SetPersistentConnection();   // 设置长连接 
        }
        /// <summary>
        /// 检查是否连接成功,通过读取PLC的M0.0固定地址来判断
        /// </summary>
        /// <returns></returns>
        public void CheckConnect()
        {
            try
            {
                if (siemensTcpNet != null)
                {
                    OperateResult<bool> res = siemensTcpNet.ReadBool("M0.0");
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
        /// <typeparam name="T"></typeparam>
        /// <param name="Addr"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool WriteValue(string Address, bool value)
        {
            try
            {
                OperateResult res = siemensTcpNet.Write(Address, value);
                return res.IsSuccess;
            }
            catch(Exception ex)
            {
                _log.WriteLog(ex.Message);
            }
            return false;
        }
        /// <summary>
        /// 向PLC写整型值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Addr"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool WriteValue(string Address, short value)
        {
            try
            {
                OperateResult res = siemensTcpNet.Write(Address, value);
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
        /// <typeparam name="T"></typeparam>
        /// <param name="Addr"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool WriteValue(string Address, int value)
        {
            try
            {
                OperateResult res = siemensTcpNet.Write(Address, value);
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
        /// <typeparam name="T"></typeparam>
        /// <param name="Addr"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool WriteValue(string Address, float value)
        {
            try
            {
                OperateResult res = siemensTcpNet.Write(Address, value);
                return res.IsSuccess;
            }
            catch (Exception ex)
            {
                _log.WriteLog(ex.Message);
            }
            return false;
        }
        /// <summary>
        /// 向PLC写文本值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Addr"></param>
        /// <param name="value"></param>
        /// <param name="WCharMode">WCharMode模式是unicode编码</param>
        /// <returns></returns>
        public bool WriteValue(string Address, string value, bool WCharMode = true, int Length=-1)
        {
            byte[] bytes;
            List<byte> list = new List<byte>();
            if (value == null) return false;

            try
            {
                if (WCharMode == true)//wchar模式
                {
                    bytes = Encoding.BigEndianUnicode.GetBytes(value);
                    list.AddRange(bytes);
                    for (int i = bytes.Length; i < Length; i++)//剩余的部分全部补齐为空格
                    {
                        if (i % 2 == 0)
                            list.Add(0);//0 32
                        else
                            list.Add(32);//0 32
                    }
                }
                else
                {
                    Encoding GB2312 = System.Text.Encoding.GetEncoding("gb2312");
                    bytes = GB2312.GetBytes(value);
                    list.Add(254);//第1个为固定字符
                    list.Add((byte)bytes.Length);//第2个为长度
                    list.AddRange(bytes);//第3个开始为内容
                }

                OperateResult res = siemensTcpNet.Write(Address, list.ToArray());

                return res.IsSuccess;
            }
            catch(Exception ex)
            {
                _log.WriteLog(ex.Message);
            }

            return false;
        }
        #endregion

        #region 读取bool值
        /// <summary>
        /// 读取标识位
        /// </summary>
        /// <param name="Address"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public bool ReadValue(string Address,out bool value)
        {
            try
            {
                OperateResult<bool> res = siemensTcpNet.ReadBool(Address);
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
        /// <summary>
        /// 批量读取标识位
        /// </summary>
        /// <param name="Address"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public bool BatchReadValue(string Address, ushort length, out bool[] value)
        {
            try
            {
                OperateResult<bool[]> res = siemensTcpNet.ReadBool(Address, length);
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

        #region 读取int16-short值
        /// <summary>
        /// 读取下位的int值
        /// </summary>
        /// <param name="Address"></param>
        /// <returns></returns>
        public bool ReadValue(string Address, out short value)
        {
            try
            {
                OperateResult<short> res = siemensTcpNet.ReadInt16(Address);
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
        public bool BatchReadValue(string Address, ushort length, out short[] value)
        {
            try
            {
                OperateResult<short[]> res = siemensTcpNet.ReadInt16(Address,length);
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

        #region 读取Int32值
        /// <summary>
        /// 读取下位的int值 dint类型
        /// </summary>
        /// <param name="Address"></param>
        /// <returns></returns>
        public bool ReadValue(string Address, out int value)
        {
            try
            {
                OperateResult<int> res = siemensTcpNet.ReadInt32(Address);
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
        public bool BatchReadValue(string Address, ushort length, out int[] value)
        {
            try
            {
                OperateResult<int[]> res = siemensTcpNet.ReadInt32(Address,length);
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
            value =null;
            return false;
        }
        #endregion

        #region 读取Float值
        /// <summary>
        /// 读取float值
        /// </summary>
        /// <param name="Address"></param>
        /// <returns></returns>
        public bool ReadValue(string Address, out float value)
        {
            try
            {
                OperateResult<float> res = siemensTcpNet.ReadFloat(Address);
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
        public bool BatchReadValue(string Address, ushort length, out float[] value)
        {
            try
            {
                OperateResult<float[]> res = siemensTcpNet.ReadFloat(Address, length);
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

        #region 读取String值
        /// <summary>
        /// 读取文本
        /// </summary>
        /// <param name="Address"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public bool ReadValue(string Address, out string value, ushort Length = 256, bool WCharMode = false)
        {
            try
            {
                OperateResult<byte[]> res = siemensTcpNet.Read(Address, Length);
                if (res.IsSuccess)
                {
                    if (WCharMode == false)//string模式读取，第1个是254，第2个是长度，第3个开始是内容
                    {
                        int count = res.Content[1];//第1个是254，第2个是长度，第3个开始是内容

                        byte[] resContent = new byte[count];
                        for (int i = 2; i < (count + 2); i++)//取出剩余的内容
                        {
                            resContent[i - 2] = res.Content[i];
                        }
                        Encoding GB2312 = Encoding.GetEncoding("gb2312");
                        value = GB2312.GetString(resContent);//按照gb2312解码
                    }
                    else//Wchar模式，其他是空格，里面是全部内容
                    {
                        StringContentFormat(res.Content);
                        value = Encoding.BigEndianUnicode.GetString(res.Content).Trim();//按照unicode解码
                    }
                    return true;
                }

            }
            catch (Exception ex)
            {
                _log.WriteLog(ex.Message);
            }
            value = "";
            return false;
        }
        private void StringContentFormat(byte[] Content)
        {
            int count = 0;
            try
            {
                for (int i = 0; i < Content.Length; i++)
                {
                    if (count < 2)
                    {
                        if (Content[i] == 0)
                        {
                            ++count;
                            if (count == 2)
                            {
                                Content[i] = 32;//0 32
                            }
                        }
                        else
                        {
                            count = 0;
                        }

                    }
                    else
                    {
                        if (i % 2 == 0)
                            Content[i] = 0;//0 32
                        else
                            Content[i] = 32;//0 32
                    }
                }
            }
            catch (Exception ex) { }
        }
        /// <summary>
        /// 按照wchar=false单个读取
        /// </summary>
        /// <param name="Address"></param>
        /// <param name="length"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool BatchReadValue(string Address, ushort length, out string[] value)
        {
            try
            {
                OperateResult<byte[]> res = siemensTcpNet.Read(Address, 1);
                if (res.IsSuccess)
                {
                    int count = res.Content[1];//第1个是254，第2个是长度，第3个开始是内容

                    byte[] resContent = new byte[count];
                    for (int i = 2; i < (count + 2); i++)//取出剩余的内容
                    {
                        resContent[i - 2] = res.Content[i];
                    }
                    Encoding GB2312 = Encoding.GetEncoding("gb2312");
                    value = new string[1];
                    value[0] = GB2312.GetString(resContent);//按照gb2312解码

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


        public bool BatchWriteValue(string Address, short[] value)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            if (siemensTcpNet != null)
            {
                siemensTcpNet.Dispose();
            }
        }

    }
    
}
