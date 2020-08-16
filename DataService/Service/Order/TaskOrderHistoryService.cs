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
    public class TaskOrderHistoryService : BaseService<TaskOrderHistory>, ITaskOrderHistoryService
    {
        public TaskOrderHistoryService(ISqlSugarClient sqlSugarClient) : base(sqlSugarClient)
        {

        }

        public bool InsertTaskOrder(TaskOrder order)
        {
            TaskOrderHistory taskOrderHistory = new TaskOrderHistory();
          
           
            bool res= Insert(taskOrderHistory);
            return res;
        }
    }
}
