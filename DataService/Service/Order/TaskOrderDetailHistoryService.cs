using iFactory.DataService.IService;
using iFactory.DataService.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFactory.DataService.Service
{
    public class TaskOrderDetailHistoryService : BaseService<TaskOrderDetailHistory>, ITaskOrderDetailHistoryService
    {
        public TaskOrderDetailHistoryService(ISqlSugarClient sqlSugarClient) : base(sqlSugarClient)
        {

        }

        public bool InsertTaskOrder(TaskOrder order)
        {
            throw new NotImplementedException();
        }
    }
}
