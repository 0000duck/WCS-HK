using System;

namespace iFactory.DevComServer
{
    /// <summary>
    /// plc类型
    /// </summary>
    public enum PLCType : int
    {
        /// <summary>
        /// 西门子1200
        /// </summary>
        Simens1200 = 1,
        /// <summary>
        /// 西门子1500
        /// </summary>
        Simens1500 = 2,
        /// <summary>
        /// 西门子300
        /// </summary>
        Simens300 = 3,
        /// <summary>
        /// 西门子200smart
        /// </summary>
        Simens200Smart = 4,
        /// <summary>
        /// 欧姆龙
        /// </summary>
        Omron = 5,
        /// <summary>
        /// 三菱
        /// </summary>
        Fx = 6,
        /// <summary>
        /// 机器人modbus连接
        /// </summary>
        Modbus = 7,
        /// <summary>
        /// 
        /// </summary>
        Undefined2 =8
    }
}
