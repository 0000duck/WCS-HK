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
        public ObservableCollection<ProductParameter> ParameterList { set; get; } = new ObservableCollection<ProductParameter>();
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
                                  IProductParameterService productParameterService)
        {
            _taskOrderService = taskOrderService;
            _taskOrderDetailService = taskOrderDetailService;
            _taskOrderHistoryService = taskOrderHistoryService;
            _taskOrderDetailHistoryService = taskOrderDetailHistoryService;
            _productParameterService = productParameterService;
        }

        public void LoadParameters()
        {
            var list = _productParameterService.QueryableToList(x => x.id > 0).OrderBy(x => x.product_name).ToList();
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    ParameterList.Add(item);
                }
            }
        }
        public void LoadAllInfos()
        {
            ModelList.Clear();
            List<TaskOrder> orders = _taskOrderService.QueryableToList(x => x.id > 0).ToList();
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

        public bool Finish(TaskOrder model)
        {
            TaskOrderHistory taskOrderHistory = new TaskOrderHistory();
            taskOrderHistory.barcode_machine_mode = model.barcode_machine_mode;
            taskOrderHistory.card_machine_enable = model.card_machine_enable;
            taskOrderHistory.end_time = DateTime.Now;
            taskOrderHistory.graphic_carton_size = model.graphic_carton_size;
            taskOrderHistory.insert_time = model.insert_time;
            taskOrderHistory.noraml_carton_size = model.noraml_carton_size;
            taskOrderHistory.open_machine_mode = model.open_machine_mode;
            taskOrderHistory.outer_carton_size = model.outer_carton_size;
            taskOrderHistory.pack_mode = model.pack_mode;
            taskOrderHistory.pallet_num = model.pallet_num;
            taskOrderHistory.pallet_size = model.pallet_size;
            taskOrderHistory.plate_enable = model.plate_enable;
            taskOrderHistory.product_count = model.product_count;
            taskOrderHistory.product_name = model.product_name;
            taskOrderHistory.product_size = model.product_size;
            taskOrderHistory.robot_pg_no = model.robot_pg_no;
            taskOrderHistory.sn_barcode_enable = model.sn_barcode_enable;
            taskOrderHistory.start_time = model.start_time;
            long id= _taskOrderHistoryService.InsertBigIdentity(taskOrderHistory);
            if(id>0)
            {
                var list = _taskOrderDetailService.QueryableToList(x=>x.order_id== model.id);
                if(list !=null && list.Count>0)
                {
                    foreach(var item in list)
                    {
                        TaskOrderDetailHistory history = new TaskOrderDetailHistory()
                        {
                            box_count = item.box_count,
                            insert_time = item.insert_time,
                            order_id = (int)id,
                            pallet_index = item.pallet_index
                        };
                        _taskOrderDetailHistoryService.Insert(history);
                    }
                }
                Remove(model);
                return true;
            }
            return false;

        }
        #endregion
    }
}
