using iFactory.DataService.Model;
using iFactoryApp.Service;
using iFactoryApp.ViewModel;
using Panuon.UI.Silver;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace iFactoryApp.View
{
    /// <summary>
    /// LineConfigView.xaml 的交互逻辑
    /// </summary>
    public partial class TaskOrderView : Page, IContextMenuView
    {
        public readonly TaskOrderViewModel viewModel;
        private readonly ISystemLogViewModel systemLogViewModel;
        private readonly TaskOrderManager _taskOrderManager;
        public TaskOrderView()
        {
            InitializeComponent();
            viewModel = IoC.GetViewModel<TaskOrderViewModel>(this);
            systemLogViewModel = IoC.Get<ISystemLogViewModel>();
            _taskOrderManager= IoC.Get<TaskOrderManager>();
            _taskOrderManager.InitialCamera(liveviewForm1, 1);
            _taskOrderManager.InitialCamera(liveviewForm2, 2);
            this.DataContext = viewModel;
            viewModel.LoadAllInfos();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CameraExecComand();
        }
        public void CameraExecComand()
        {
            for (int i = 0; i < _taskOrderManager.cameraList.Count; i++)
            {
                _taskOrderManager.cameraList[i].ExecCommandLon();//触发相机
            }
        }
        private void ContextMenu_Click(object sender, RoutedEventArgs e)
        {
            MenuItem contextMenu = e.Source as MenuItem;
            if (contextMenu.Name == "New")//新建
            {
                ContextMenu_Click(contextMenu.Name);
            }
            else if (contextMenu.Name == "Edit")//编辑
            {
                ContextMenu_Click(contextMenu.Name);
            }
            else if (contextMenu.Name == "Delete")//删除
            {
                ContextMenu_Click(contextMenu.Name);
            }
            else if (contextMenu.Name == "Finish")//删除
            {
                ContextMenu_Click(contextMenu.Name);
            }
        }
        public void ContextMenu_Click(string click_name)
        {
            if (click_name == "New")//新建
            {
                viewModel.EditModel = new TaskOrder();
                TaskOrderEditView editView = new TaskOrderEditView();
                editView.Topmost = true;
                editView.Show();
                editView.Closed += EditView_Closed;
            }
            else if (click_name == "Edit")//编辑
            {
                if (viewModel.SelectedModel != null)
                {
                    viewModel.EditModel = viewModel.SelectedModel;//当前编辑对象
                    if (viewModel.EditModel.product_count <= 0)
                    {
                        TaskOrderEditView editView = new TaskOrderEditView();
                        editView.Topmost = true;
                        editView.Show();
                        editView.Closed += EditView_Closed;
                    }
                    else
                    {
                        MessageBoxX.Show("任务单仅在未生产前进行编辑！", "错误", Application.Current.MainWindow);
                    }
                }
                else
                {
                    MessageBoxX.Show("请先选择一个任务单进行操作！", "错误", Application.Current.MainWindow);
                }
            }
            else if (click_name == "Delete")//删除
            {
                if (viewModel.SelectedModel != null)
                {
                    MessageBoxResult result = MessageBoxX.Show($"确定删除任务吗？", "确认", Application.Current.MainWindow, MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        if (viewModel.Remove(viewModel.SelectedModel) == false)
                        {
                            MessageBoxX.Show("任务删除失败！", "错误", Application.Current.MainWindow);
                        }
                    }
                }
                else
                {
                    MessageBoxX.Show("请先选择一个任务单进行操作！", "错误", Application.Current.MainWindow);
                }
            }
            else if (click_name == "Finish")//完成
            {
                if (viewModel.SelectedModel != null)
                {
                    TaskOrderFinishView taskOrderFinishView = new TaskOrderFinishView();
                    taskOrderFinishView.ShowDialog();
                }
                else
                {
                    MessageBoxX.Show("当前未有任务信息，不能完成！", "错误", Application.Current.MainWindow);
                }
            }
            else if (click_name == "Download")//下载参数
            {
                if (viewModel.SelectedModel != null)
                {
                    MessageBoxResult result = MessageBoxX.Show($"确定再次下载参数吗？", "确认", Application.Current.MainWindow, MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        if(!_taskOrderManager.StartToDownloadParamter(viewModel.SelectedModel))
                        {
                            MessageBoxX.Show("参数下载失败！", "错误", Application.Current.MainWindow);
                        }
                    }
                }
                else
                {
                    MessageBoxX.Show("当前未有任务信息，不能完成！", "错误", Application.Current.MainWindow);
                }
            }
        }
        //编辑窗体关闭刷新
        private void EditView_Closed(object sender, EventArgs e)
        {
            this.DataContext = null;
            viewModel.LoadAllInfos();
            this.DataContext = viewModel;
        }

    }
}
