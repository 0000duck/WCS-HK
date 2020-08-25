using HslCommunication;
using HslCommunication.ModBus;
using iFactory.CommonLibrary;
using iFactory.CommonLibrary.Interface;
using System;
using System.Threading;

namespace iFactory.DevComServer.Tcp
{
    public class RobotModbus:IDisposable
    {
        private ModbusTcpNet _robotModbus = null;
        private string _mSzIp = null;
        private static readonly int _mIPort = 502;
        private Thread _WorkTh;
        private byte _station;
        private readonly ILogWrite _log;

        public RobotModbus(ILogWrite logWrite)
        {
            _log = logWrite;
            _WorkTh = new Thread(Back_work);
            _WorkTh.IsBackground = true;
            _WorkTh.Start();
        }
        public void Connect(string ip=null, byte station = 0)
        {
            _station = station;

            try
            {
                if(!string.IsNullOrEmpty(ip))
                {
                    _mSzIp = ip;
                }
                _robotModbus?.ConnectClose();
                _robotModbus?.Dispose();
                _robotModbus = new ModbusTcpNet(_mSzIp, _mIPort, _station);
                _robotModbus.AddressStartWithZero = true;//首地址从0开始
                _robotModbus.DataFormat = HslCommunication.Core.DataFormat.ABCD;
                _robotModbus.IsStringReverse = false; //字符串跌倒
                _robotModbus.SetPersistentConnection();
                _log.WriteLog($"modbus连接成功{_mSzIp}");

            }
            catch (Exception ex)
            {
                _log.WriteLog($"modbus连接失败{_mSzIp}",ex);
            }
           
        }

        #region 线程执行代码
        private void Back_work(object o)
        {
            while (true)
            {
                if (CheckConnect())
                {

                }
                else
                {
                    if (NetworkHelper.IsNetWorkConnect(_mSzIp))//plc可以ping通
                    {
                        Thread.Sleep(1000);
                        _log.WriteLog($"modbus连接中断，开始重新连接{_mSzIp}");
                        Connect();
                    }
                    else
                    {
                        _log.WriteLog("modbus连接中断，等待再次连接！");
                        Thread.Sleep(5000);//延时等待PLC再次连接
                    }
                }
                Thread.Sleep(3000);
            }
        }
        #endregion

        #region 检查是否要断线重连
        public bool CheckConnect()
        {
            if(_robotModbus !=null)
            {
                bool res = false;
                return Discan(0, out res);
            }
            return false;
        }
        #endregion

        #region 写多个寄存器
        /// <summary>
        ///short int型寄存器
        /// </summary>
        /// <param name="startaddress">The register address(The base is 1).</param>
        /// <param name="data">The register data. The value is from 0~65535.</param>
        /// <returns></returns>
        public bool Write_MutiRegs(int startaddress, short[] data)
        {
            if (_robotModbus.Write(startaddress.ToString(), data).IsSuccess)
                return true;
            else
                return false;
        }
        #endregion

        #region 写多个寄存器
        /// <summary>
        /// 浮点型寄存器
        /// </summary>
        /// <param name="startaddress"></param>
        /// <param name="data">The array of data for setting registers.</param>
        /// <returns></returns>
        public bool Write_MutiRegs(int startaddress, float[] data)
        {
            if (_robotModbus.Write(startaddress.ToString(), data).IsSuccess)
                return true;
            else
                return false;
        }
        #endregion

        #region 写单个寄存器
        /// <summary>
        ///short int型寄存器
        /// </summary>
        /// <param name="startaddress">The register address(The base is 1).</param>
        /// <param name="data">The register data. The value is from 0~65535.</param>
        /// <returns></returns>
        public bool Write_SignlReg(int startaddress, short data)
        {
            if (_robotModbus.Write(startaddress.ToString(), data).IsSuccess)
                return true;
            else
                return false;
        }
        #endregion

        #region 读单个寄存器
        /// <summary>
        /// short int型寄存器
        /// </summary>
        /// <param name="startaddress">The register address(The base is 1).</param>
        /// <param name="data">The register data.</param>
        /// <returns></returns>
        public bool Read_SignlReg(int startaddress, out short data)
        {
            OperateResult<short> result = _robotModbus.ReadInt16(startaddress.ToString());
            if (result.IsSuccess)
            {
                data = result.Content;
                return true;
            }
            else
            {
                data = 0;
                return false;
            }

        }
        #endregion

        #region 读多个寄存器
        /// <summary>
        ///short int型寄存器
        /// </summary>
        /// <param name="startaddress">The register address(The base is 1).</param>
        /// <param name="data">The register data.</param>
        /// <returns></returns>
        public bool Read_MutiRegs(int startaddress, ushort totalnums, out short[] data)
        {
            OperateResult<short[]> result = _robotModbus.ReadInt16(startaddress.ToString(), totalnums);
            if (result.IsSuccess)
            {
                data = result.Content;
                return true;
            }
            else
            {
                data = null;
                return false;
            }
        }
        #endregion

        #region 读多个寄存器
        /// <summary>
        /// float型寄存器
        /// </summary>
        /// <param name="startaddress">The register address(The base is 1).</param>
        /// <param name="data">The register data.</param>
        /// <returns></returns>
        public bool Read_MutiRegs(int startaddress, ushort totalnums, out float[] data)
        {
            OperateResult<float[]> result = _robotModbus.ReadFloat(startaddress.ToString(), totalnums);
            if (result.IsSuccess)
            {
                data = result.Content;
                return true;
            }
            else
            {
                data = null;
                return false;
            }

        }
        #endregion

        #region Signl DO Set
        public bool DoSetValue(int address, bool data)
        {
            if (_robotModbus.WriteCoil((16 + address).ToString(), data).IsSuccess)
                return true;
            else
                return false;
        }
        #endregion

        #region Muti DO Set
        public bool DoSetValue(int startaddress, bool[] data)
        {
            if (_robotModbus.WriteCoil((16 + startaddress).ToString(), data).IsSuccess)
                return true;
            else
                return false;
        }
        #endregion

        #region 读单个Di的值
        public bool Discan(int address, out bool DoData)
        {
            OperateResult<bool> result = _robotModbus.ReadCoil(address.ToString());
            if (result.IsSuccess)
            {
                DoData = result.Content;
                return true;
            }
            else
            {
                DoData = false;
                return false;
            }
        }
        #endregion

        #region 读多个Di的值
        public bool Discan(int startaddress, ushort totalnums, out bool[] DoData)
        {
            OperateResult<bool[]> result = _robotModbus.ReadCoil(startaddress.ToString(), totalnums);
            if (result.IsSuccess)
            {
                DoData = result.Content;
                return true;
            }
            else
            {
                DoData = new bool[totalnums];
                return false;
            }
        }


        #endregion

        public void Dispose()
        {
            _robotModbus?.Dispose();
            _WorkTh?.Abort();
        }
    }
}
