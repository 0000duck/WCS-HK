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
    /// 历史任务单信息
    /// </summary>
    [SugarTable("task_order_detail_history")]
    public class TaskOrderDetailHistory : TaskOrderDetail
    {
    }
}
