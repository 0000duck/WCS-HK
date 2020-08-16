using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFactory.DataService.Model
{
    /// <summary>
    /// 已完成的任务单信息
    /// </summary>
    [SugarTable("task_order_history")]
    public class TaskOrderHistory : TaskOrder
    {
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime end_time { set; get; }
    }
}
