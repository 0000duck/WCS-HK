using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace iFactory.CommonLibrary
{
    /// <summary>
    /// 开盒设备模式
    /// </summary>
    public enum OpenMachineMode : int
    {
        [Description("无")]
        None = 0,
        [Description("普通箱")]
        Normal = 1,
        [Description("彩箱")]
        Graphic = 2
    }
    /// <summary>
    /// 贴标设备模式
    /// </summary>
    public enum BarcodeMachineMode : int
    {
        [Description("无")]
        None = 0,
        [Description("1#彩盒贴标")]
        Barcode1 = 1,
        [Description("2#彩盒贴标")]
        Barcode2 = 2
    }
    /// <summary>
    /// 贴标设备模式
    /// </summary>
    public enum PackMode : int
    {
        [Description("不装箱模式")]
        None = 0,
        [Description("装箱模式")]
        Pack = 1
    }
    /// <summary>
    /// 设备启用模式
    /// </summary>
    public enum EnableMode : int
    {
        [Description("不使用")]
        Disable = 0,
        [Description("使用")]
        Enable = 1
    }
}
