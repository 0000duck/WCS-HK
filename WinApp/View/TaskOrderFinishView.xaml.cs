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
    public partial class TaskOrderFinishView : Window
    {
        private readonly TaskOrderViewModel _viewModel;

        public TaskOrderFinishView()
        {
            InitializeComponent();
            _viewModel = IoC.GetViewModel<TaskOrderViewModel>(this);
            this.DataContext = _viewModel;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.Finish(_viewModel.SelectedModel) == false)
            {
                MessageBoxX.Show("任务完成失败！", "错误", Application.Current.MainWindow);
            }
            else
            {
                _viewModel.SelectedModel = null;
            }

            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
