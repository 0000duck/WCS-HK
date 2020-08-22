using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFactory.DataService.Model
{
    [SugarTable("task_order_detail")]
    public class TaskOrderDetail: BaseNotifyModel
    {
        public int order_id { set; get; }
        private int _pallet_index;
        /// <summary>
        /// 托盘序号
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int pallet_index
        {
            set
            {
                _pallet_index = value;
                NotifyPropertyChanged("pallet_index");
            }
            get { return _pallet_index; }
        }

        private int _box_count;
        /// <summary>
        /// 盒子数量
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int box_count
        {
            set
            {
                _box_count = value;
                NotifyPropertyChanged("box_count");
            }
            get { return _box_count; }
        }

        private DateTime _insert_time;
        /// <summary>
        /// 数据插入时间
        /// </summary>
        [SugarColumn]
        public DateTime insert_time
        {
            set
            {
                _insert_time = value;
                NotifyPropertyChanged("insert_time");
            }
            get { return _insert_time; }
        }
    }
}
