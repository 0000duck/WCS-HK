using iFactory.CommonLibrary;
using iFactory.DataService.IService;
using iFactory.DataService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFactoryApp.ViewModel
{
    public class TaskOrderViewModel
    {
        private readonly ITaskOrderService _taskOrderService;
        private readonly ITaskOrderHistoryService _taskOrderHistoryService;
        private readonly ITaskOrderDetailService _taskOrderDetailService;
        private readonly ITaskOrderDetailHistoryService _taskOrderDetailHistoryService;
        private readonly IProductParameterService _productParameterService;
        
        public ObservableCollection<TaskOrder> ModelList { set; get; } = new ObservableCollection<TaskOrder>();
        public TaskOrder EditModel { set; get; }
        public TaskOrder SelectedModel { set; get; }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public TaskOrderViewModel(ITaskOrderService taskOrderService,
                                   ITaskOrderHistoryService taskOrderHistoryService,
                                   ITaskOrderDetailService taskOrderDetailService,
                                   ITaskOrderDetailHistoryService taskOrderDetailHistoryService,
                                   IProductParameterService productParameterService
                                    )
        {
            _taskOrderService = taskOrderService;
            _taskOrderDetailService = taskOrderDetailService;
            _taskOrderHistoryService = taskOrderHistoryService;
            _taskOrderDetailHistoryService = taskOrderDetailHistoryService;
            _productParameterService = productParameterService;
           
            LoadAllInfos();
        }
        public void LoadAllInfos()
        {
            List<TaskOrder> orders = _taskOrderService.QueryableToList(x => x.id > 0 && x.order_status < (int)OrderStatusEnum.Stoping).ToList();
            foreach (var item in orders)
            {
                ModelList.Add(item);
            }
        }
        #region 对象操作
        public bool Insert(TaskOrder model)
        {
            long id = _taskOrderService.InsertBigIdentity(model);
            if (id > 0)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="IsUpdateDetails">更新明细</param>
        /// <returns></returns>
        public bool Update(TaskOrder model, bool IsUpdateDetails = true)
        {
            if (_taskOrderService.UpdateEntity(model))
            {
                if (IsUpdateDetails)
                {
                    _taskOrderDetailService.Delete(x => x.order_id == model.id);//删除明细
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
        public bool Remove(TaskOrder model, bool RemoveListOnly = false)
        {
            if (RemoveListOnly)
            {
                if (ModelList.Remove(model))
                {
                    return true;
                }
            }
            else
            {
                if (_taskOrderService.IsAny(x => x.id == model.id))
                {
                    if (_taskOrderService.Delete(x => x.id == model.id))
                    {
                        _taskOrderDetailService.Delete(x => x.order_id == model.id);//删除明细
                        if (ModelList.Remove(model))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
        #endregion
    }
}
