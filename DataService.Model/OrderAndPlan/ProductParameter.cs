using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFactory.DataService.Model
{
    [SugarTable("product_parameter")]
    public class ProductParameter : BaseNotifyModel
    {
        /// <summary>
        /// 产品名称
        /// </summary>
        private string _product_name ;
        public string product_name
        {
            set
            {
                _product_name = value;
                NotifyPropertyChanged("product_name");
            }
            get { return _product_name; }
        }
       
        private string _product_size;
        /// <summary>
        /// 产品尺寸
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 100)]
        public string product_size
        {
            set
            {
                _product_size = value;
                NotifyPropertyChanged("product_size");
            }
            get { return _product_size; }
        }

        private string _graphic_carton_size;
        /// <summary>
        /// 彩盒尺寸
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 50)]
        public string graphic_carton_size
        {
            set
            {
                _graphic_carton_size = value;
                NotifyPropertyChanged("graphic_carton_size");
            }
            get { return _graphic_carton_size; }
        }
        private string _noraml_carton_size;
        /// <summary>
        /// 普通箱尺寸
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 50)]
        public string noraml_carton_size
        {
            set
            {
                _noraml_carton_size = value;
                NotifyPropertyChanged("noraml_carton_size");
            }
            get { return _noraml_carton_size; }
        }
        private string _outer_carton_size;
        /// <summary>
        /// 外箱尺寸
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 50)]
        public string outer_carton_size
        {
            set
            {
                _outer_carton_size = value;
                NotifyPropertyChanged("outer_carton_size");
            }
            get { return _outer_carton_size; }
        }
        private string _pallet_size;
        /// <summary>
        /// 托盘尺寸
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 50)]
        public string pallet_size
        {
            set
            {
                _pallet_size = value;
                NotifyPropertyChanged("pallet_size");
            }
            get { return _pallet_size; }
        }
        private int _robot_pg_no;
        /// <summary>
        /// 垛型，机械手程序号
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int robot_pg_no
        {
            set
            {
                _robot_pg_no = value;
                NotifyPropertyChanged("robot_pg_no");
            }
            get { return _robot_pg_no; }
        }
        private int _pallet_num;
        /// <summary>
        /// 托盘数量
        /// </summary>
        [SugarColumn]
        public int pallet_num
        {
            set
            {
                _pallet_num = value;
                NotifyPropertyChanged("pallet_num");
            }
            get { return _pallet_num; }
        }
       
        private int _open_machine_mode;
        /// <summary>
        /// 开盒设备模式。1=普通箱、2=彩箱，=0都不使用
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int open_machine_mode
        {
            set
            {
                _open_machine_mode = value;
                NotifyPropertyChanged("open_machine_mode");
            }
            get { return _open_machine_mode; }
        }
        private int _barcode_machine_mode;
        /// <summary>
        ///贴标设备模式 1=1#彩盒贴标，2=2#彩盒贴标；=0都不会使用
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int barcode_machine_mode
        {
            set
            {
                _barcode_machine_mode = value;
                NotifyPropertyChanged("barcode_machine_mode");
            }
            get { return _barcode_machine_mode; }
        }
        private bool _sn_barcode_enable = false;
        /// <summary>
        /// sn条码检测是否启用
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public bool sn_barcode_enable
        {
            set
            {
                _sn_barcode_enable = value;
                NotifyPropertyChanged("sn_barcode_enable");
            }
            get { return _sn_barcode_enable; }
        }
        private bool _card_machine_enable = false;
        /// <summary>
        /// 发卡机是否启用
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public bool card_machine_enable
        {
            set
            {
                _card_machine_enable = value;
                NotifyPropertyChanged("card_machine_enable");
            }
            get { return _card_machine_enable; }
        }
        private bool _plate_enable = false;
        /// <summary>
        /// 隔板是否启用
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public bool plate_enable
        {
            set
            {
                _plate_enable = value;
                NotifyPropertyChanged("plate_enable");
            }
            get { return _plate_enable; }
        }
    }
}
