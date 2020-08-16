using HslCommunication;
using HslCommunication.ModBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace iFactory.DevComServer
{
    public class ModbusTcpHelper
    {
        private ModbusTcpNet _deviceModbus = null;
        private string _mSzIp = null;
        private static int _mIPort = 502;
        private Thread _WorkTh;
        private byte _station;
        public bool IsConnected { set; get; }

        public OperateResult Connect { private set; get; }

        public ModbusTcpHelper(string ip,int port, byte station = 0)
        {
            _mSzIp = ip;
            _mIPort = port;
            _station = station;

            ModbusObjectInitial();
            Connect = _deviceModbus.ConnectServer();

            _WorkTh = new Thread(Back_work);
            _WorkTh.IsBackground = true;
            _WorkTh.Start();
        }
        public void ModbusObjectInitial()
        {
            _deviceModbus?.ConnectClose();
            _deviceModbus = new ModbusTcpNet(_mSzIp, _mIPort, _station);
            _deviceModbus.AddressStartWithZero = true;//首地址从0开始
            _deviceModbus.DataFormat = HslCommunication.Core.DataFormat.ABCD;
            _deviceModbus.IsStringReverse = false; //字符串跌倒
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
                    ModbusObjectInitial();
                    Thread.Sleep(1000);
                    _deviceModbus.ConnectServer();//重新连接
                }
                Thread.Sleep(3000);
            }
        }
        #endregion

        #region 检查是否要断线重连
        public bool CheckConnect()
        {
            short data;
            if (Read_SignlReg(40003,out data))//读取周期地址成功
            {
                this.IsConnected = true;
                return true;
            }
            else
            {
                this.IsConnected = false;
                return false;
            }
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
            if (_deviceModbus.Write(startaddress.ToString(), data).IsSuccess)
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
            if (_deviceModbus.Write(startaddress.ToString(), data).IsSuccess)
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
            if (_deviceModbus.Write(startaddress.ToString(), data).IsSuccess)
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
            OperateResult<short> result = _deviceModbus.ReadInt16(startaddress.ToString());
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
            OperateResult<short[]> result = _deviceModbus.ReadInt16(startaddress.ToString(), totalnums);
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
            OperateResult<float[]> result = _deviceModbus.ReadFloat(startaddress.ToString(), totalnums);
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
        public bool ADAMDoSetValue(int address, bool data)
        {
            if (_deviceModbus.WriteCoil((16 + address).ToString(), data).IsSuccess)
                return true;
            else
                return false;
        }
        #endregion

        #region Muti DO Set
        public bool ADAMDoSetValue(int startaddress, bool[] data)
        {
            if (_deviceModbus.WriteCoil((16 + startaddress).ToString(), data).IsSuccess)
                return true;
            else
                return false;
        }
        #endregion

        #region 读单个Di的值
        public bool ADAMDiscan(int address, out bool DoData)
        {
            OperateResult<bool> result = _deviceModbus.ReadCoil(address.ToString());
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
        public bool ADAMDiscan(int startaddress, ushort totalnums, out bool[] DoData)
        {
            OperateResult<bool[]> result = _deviceModbus.ReadCoil(startaddress.ToString(), totalnums);
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
    }
}
