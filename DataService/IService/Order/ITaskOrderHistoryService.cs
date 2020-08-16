using iFactory.DataService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFactory.DataService.IService
{
    public interface ITaskOrderHistoryService : IBaseService<TaskOrderHistory>
    {
        bool InsertTaskOrder(TaskOrder order);
    }
}
