using iFactory.CommonLibrary;
using iFactory.DataService.IService;
using iFactory.DataService.Model;
using iFactory.DevComServer;
using iFactoryApp.Service;
using iFactoryApp.ViewModel;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace iFactoryApp.View
{
    /// <summary>
    /// LineConfigView.xaml 的交互逻辑
    /// </summary>
    public partial class TaskOrderEditView : Window
    {
        private readonly TaskOrderManager _taskOrderManager;
        private readonly TaskOrderViewModel _viewModel;
        private readonly IProductParameterService _productParameterService;

        public TaskOrderEditView()
        {
            InitializeComponent();
            _viewModel = IoC.GetViewModel<TaskOrderViewModel>(this);
            _productParameterService = IoC.Get<IProductParameterService>();
            _taskOrderManager= IoC.Get<TaskOrderManager>();
            _viewModel.LoadParameters();
            this.DataContext = _viewModel;
            if (_viewModel.EditModel.pack_mode == (int)PackMode.Pack)//装箱模式
            {
                radioButton1.IsChecked = true;
            }
            else
            {
                radioButton2.IsChecked = true;
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_viewModel.EditModel.product_name))
            {
                MessageBoxX.Show("产品名称不能为空", "错误", Application.Current.MainWindow);
                return;
            }
            var parameter= _productParameterService.QueryableToEntity(x => x.product_name== _viewModel.EditModel.product_name);
            if(parameter !=null)
            {   
                _viewModel.EditModel.barcode_machine_mode = parameter.barcode_machine_mode;
                _viewModel.EditModel.card_machine_enable = parameter.card_machine_enable;
                _viewModel.EditModel.graphic_carton_size = parameter.graphic_carton_size;
                _viewModel.EditModel.noraml_carton_size = parameter.noraml_carton_size;
                _viewModel.EditModel.open_machine_mode = parameter.open_machine_mode;
                _viewModel.EditModel.outer_carton_size = parameter.outer_carton_size;
                _viewModel.EditModel.pallet_num = parameter.pallet_num;
                _viewModel.EditModel.pallet_size = parameter.pallet_size;
                _viewModel.EditModel.plate_enable = parameter.plate_enable;
                _viewModel.EditModel.product_name = parameter.product_name;
                _viewModel.EditModel.product_size = parameter.product_size;
                _viewModel.EditModel.robot_pg_no = parameter.robot_pg_no;
                _viewModel.EditModel.sn_barcode_enable = parameter.sn_barcode_enable;
            }
            _viewModel.EditModel.order_status =(int) OrderStatusEnum.Running;
            _viewModel.EditModel.start_time = DateTime.Now;
            _viewModel.EditModel.pack_mode = (int)PackMode.None;
            if (radioButton1.IsChecked==true)//装箱模式
            {
                _viewModel.EditModel.pack_mode = (int)PackMode.Pack;
            }
            if (_viewModel.EditModel.id > 0)
            {
                if(!_viewModel.Update(_viewModel.EditModel))
                {
                    MessageBoxX.Show("任务单信息保存失败", "错误", Application.Current.MainWindow);
                }
                else
                {
                    _taskOrderManager.StartToDownloadParamter(_viewModel.EditModel);
                }
            }
            else
            {
                if(!_viewModel.Insert(_viewModel.EditModel))
                {
                    MessageBoxX.Show("任务单信息保存失败", "错误", Application.Current.MainWindow);
                }
                else
                {
                    _taskOrderManager.StartToDownloadParamter(_viewModel.EditModel);
                }
            }

            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
