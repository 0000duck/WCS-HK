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
    /// 订单状态
    /// </summary>
    public enum OrderStatusEnum : int
    {
        [Description("已创建")]
        Created = 0,
        [Description("启动中")]
        Starting = 1,
        [Description("运行中")]
        Running = 2,
        [Description("停止中")]
        Stoping = 3,
        [Description("已完成")]
        Completed=4,
        [Description("取消中")]
        Canceling = 5,
        [Description("已取消")]
        Canceled = 6,
        [Description("错误")]
        Error = 7,
        [Description("换仓中")]
        ChangeBins = 8,
        [Description("仓满停机")]
        BinFullStop = 9
    }

    /// <summary>
    /// 任务单命令
    /// </summary>
    public enum TaskCommandEnum : int
    {
        /// <summary>
        /// 完成进行的工单，并切换至下一个
        /// </summary>
        [Description("无")]
        FinishAndSwitch = 0,
        [Description("启动")]
        Start = 1,
        [Description("停止")]
        Stop = 2,
        [Description("重启")]
        Restart = 3,
        [Description("急停取消")]
        EstopAndCancel = 4,
        [Description("在线换仓")]
        ChangeDest = 5
    }
}
