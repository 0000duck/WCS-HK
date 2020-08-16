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
    /// 线别类型
    /// </summary>
    public enum LineType : int
    {
        [Description("筒仓接收")]
        SiloReceiving = 0,
        [Description("原料接收")]
        Receiving = 1,
        [Description("粉碎工段")]
        Grinding = 2,
        [Description("配料工段")]
        Batching = 3,
        [Description("制粒工段")]
        Pelleting = 4,
        [Description("膨化工段")]
        Extrusion = 5,
        [Description("包装工段")]
        Packing = 6,
        [Description("公共工段")]
        Common = 7
    }
    /// <summary>
    /// 设备类型
    /// </summary>
    public enum DeviceType : int
    {
        [Description("料仓")]
        Bin = 0,
        [Description("投料口卸料点")]
        DumpPlace = 1,
        [Description("显示终端")]
        MonitorTerminal = 2,
        [Description("流程设备")]
        ProcessDevice = 3
    }
    /// <summary>
    /// 路径类型
    /// </summary>
    public enum RouteType : int
    {
        [Description("路径源")]
        Source = 0,
        [Description("路径目标")]
        Dest = 1
    }
    /// <summary>
    /// 设备当前状态
    /// </summary>
    public enum DeviceStatus : int
    {
        [Description("正常可用的")]
        Available = 0,
        [Description("保养或检修")]
        Maintenance = 1,
        [Description("故障")]
        Error = 2
    }
    /// <summary>
    /// 线别地址信息
    /// </summary>
    public enum LineAddrType
    {
        [Description("路径命令")]
        route_command =0,
        [Description("路径当前状态")]
        route_status = 1,
        [Description("源仓位")]
        route_source = 2,
        [Description("目标仓位")]
        route_dest = 3,
        [Description("源仓低料位")]
        source_low_level = 4,
        [Description("目标仓高料位")]
        dest_high_level = 5,
        [Description("路径选项1")]
        route_option1 = 6,
        [Description("路径选项2")]
        route_option2 = 7
    }
}
