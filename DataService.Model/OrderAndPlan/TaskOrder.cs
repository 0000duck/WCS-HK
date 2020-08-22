using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFactory.DataService.Model
{
    [SugarTable("task_order")]
    public class TaskOrder: ProductParameter
    {
        private int _pack_mode = 0;
        [SugarColumn]
        public int pack_mode
        {
            set
            {
                _pack_mode = value;
                NotifyPropertyChanged("pack_mode");
            }
            get { return _pack_mode; }
        }
        private DateTime _insert_time=DateTime.Now;
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
        private DateTime _start_time;
        /// <summary>
        /// 开始时间
        /// </summary>
        [SugarColumn]
        public DateTime start_time
        {
            set
            {
                _start_time = value;
                NotifyPropertyChanged("start_time");
            }
            get { return _start_time; }
        }
        private int _order_status=0;
        /// <summary>
        /// 任务状态
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int order_status
        {
            set
            {
                _order_status = value;
                NotifyPropertyChanged("order_status");
            }
            get { return _order_status; }
        }
        private int _product_count=0;
        /// <summary>
        /// 完成数量
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int product_count
        {
            set
            {
                _product_count = value;
                NotifyPropertyChanged("product_count");
            }
            get { return _product_count; }
        }
    }
}
