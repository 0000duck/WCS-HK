﻿using iFactory.CommonLibrary;
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
            this.DataContext = viewModel;
            viewModel.LoadAllInfos();
            _taskOrderManager.InitialCamera(liveviewForm1, 1);
            _taskOrderManager.InitialCamera(liveviewForm1, 2);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < _taskOrderManager.cameraList.Count; i++)
            {
                _taskOrderManager.cameraList[i].ExecCommandLon();//触发相机
            }
        }
        private void Datagrid1_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void datagrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            viewModel.SelectedModel = null;
          
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
            else if (click_name == "Finish")//删除
            {
                if (viewModel.SelectedModel != null)
                {
                    MessageBoxResult result = MessageBoxX.Show($"确定完成任务吗？", "确认", Application.Current.MainWindow, MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        if (viewModel.Finish(viewModel.SelectedModel) == false)
                        {
                            MessageBoxX.Show("任务完成失败！", "错误", Application.Current.MainWindow);
                        }
                        else
                        {
                            viewModel.SelectedModel = null;
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
