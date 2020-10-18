using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFactory.DevComServer
{
    /// <summary>
    /// 地址偏移量
    /// </summary>
    public class AddrOffset
    {
        public AddrOffset(bool Continuous, int Offset)
        {
            this.IsContinuous = Continuous;
            this.OffsetLength = Offset;
        }
        /// <summary>
        /// 是否连续
        /// </summary>
        public bool IsContinuous { set; get; }
        /// <summary>
        /// 偏移长度
        /// </summary>
        public int OffsetLength { set; get; }
    }
    /// <summary>
    /// 地址连续检查
    /// </summary>
    public class AddressOffsetCheck
    {
        /// <summary>
        /// 标签地址连续性判断
        /// </summary>
        /// <returns>地址连续返回True,否则返回False</returns>
        public static AddrOffset TagContinuousCheck<T>(Tag<T> previousTag, Tag<T> currentTag, PLCType plcType)
        {
            AddrOffset addrOffset = new AddrOffset(true, 1);
            switch (plcType)
            {
                case PLCType.Simens1200:
                case PLCType.Simens1500:
                case PLCType.Simens300:
                case PLCType.Simens200Smart:
                    return SimimensTagContinuousCheck(previousTag, currentTag);
                case PLCType.Omron:
                case PLCType.Fx:
                    return OmronFxTagContinuousCheck(previousTag, currentTag);
                case PLCType.Modbus:
                    return ModbusContinuousCheck(previousTag, currentTag);
            }
            return addrOffset;
        }
        /// <summary>
        /// 西门子标签地址连续性判断
        /// </summary>
        /// <returns></returns>
        public static AddrOffset SimimensTagContinuousCheck<T>(Tag<T> previousTag, Tag<T> currentTag)
        {
            AddrOffset addrOffset = new AddrOffset(true, 1);
            string addr1Str, addr2Str;
            string[] addr1Array, addr2Array;
            double addr1D, addr2D, addrSpan;
            addr1Str = previousTag.TagAddr.Replace("DB", String.Empty);
            addr2Str = currentTag.TagAddr.Replace("DB", String.Empty);
            addr1Str = addr1Str.Replace("db", String.Empty);
            addr2Str = addr2Str.Replace("db", String.Empty);

            addr1Array = addr1Str.Split('.');
            addr2Array = addr2Str.Split('.');

            if (currentTag.DataType == TagDataType.Bool)//DB1.0.0  DB1.0.1
            {
                if (addr1Array.Length == 3 && addr2Array.Length == 3)
                {
                    double.TryParse(addr1Array[2], out addr1D);
                    double.TryParse(addr2Array[2], out addr2D);
                    addrSpan = addr2D - addr1D;
                    if (addrSpan == 1)
                    {
                        addrOffset.OffsetLength = (int)addrSpan;
                        return addrOffset;
                    }
                }
            }
            else if (currentTag.DataType == TagDataType.Short)//DB1.10 DB1.12
            {
                if (addr1Array.Length == 2 && addr2Array.Length == 2)
                {
                    double.TryParse(addr1Array[1], out addr1D);
                    double.TryParse(addr2Array[1], out addr2D);
                    addrSpan = addr2D - addr1D;
                    if (addrSpan <= 10)//地址间隔10个长度以内
                    {
                        addrOffset.OffsetLength = (int)(addrSpan/2);
                        return addrOffset;
                    }
                }
            }
            else if (currentTag.DataType == TagDataType.Int || currentTag.DataType == TagDataType.Float)//DB1.1092 DB1.1096
            {
                if (addr1Array.Length == 2 && addr2Array.Length == 2)
                {
                    double.TryParse(addr1Array[1], out addr1D);
                    double.TryParse(addr2Array[1], out addr2D);
                    addrSpan = addr2D - addr1D;
                    if (addrSpan == 4)
                    {
                        addrOffset.OffsetLength = (int)(addrSpan/4);
                        return addrOffset;
                    }
                }
            }
            return addrOffset;
        }
        /// <summary>
        /// 欧姆龙标签地址连续性判断
        /// </summary>
        /// <param name="previousTag"></param>
        /// <param name="currentTag"></param>
        /// <returns>地址连续返回True,否则返回False.第二个参数返回间隔</returns>
        public static AddrOffset OmronFxTagContinuousCheck<T>(Tag<T> previousTag, Tag<T> currentTag)
        {
            AddrOffset addrOffset = new AddrOffset(true, 1);
            string addr1Str, addr2Str;
            int addr1D, addr2D, addrSpan;
            addr1Str = previousTag.TagAddr.Replace("D", String.Empty);
            addr2Str = currentTag.TagAddr.Replace("D", String.Empty);
            addr1Str = addr1Str.Replace("d", String.Empty);
            addr2Str = addr2Str.Replace("d", String.Empty);

            if (currentTag.DataType == TagDataType.Bool)//
            {
                return new AddrOffset(false, 0);
            }
            else if (currentTag.DataType == TagDataType.Short)//D100 D102
            {
                int.TryParse(addr1Str, out addr1D);
                int.TryParse(addr2Str, out addr2D);
                addrSpan = addr2D - addr1D;
                if (addrSpan <= 10)//间隔10个以内
                {
                    addrOffset.OffsetLength = (int)(addrSpan / 2);
                    return addrOffset;
                }
            }
            else if (currentTag.DataType == TagDataType.Int || currentTag.DataType == TagDataType.Float)//DB1.1092 DB1.1096
            {

            }
            return addrOffset;
        }
        /// <summary>
        ///Modbus地址连续性判断
        /// </summary>
        /// <returns>地址连续返回True,否则返回False</returns>
        public static AddrOffset ModbusContinuousCheck<T>(Tag<T> previousTag, Tag<T> currentTag)
        {
            AddrOffset addrOffset = new AddrOffset(true, 1);
            return addrOffset;
        }
    }
}
