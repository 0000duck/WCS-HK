using iFactory.CommonLibrary;
using iFactory.DataService.Model;
using iFactory.DevComServer;
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

        public TaskOrderView()
        {
            InitializeComponent();
            ProductParameterView productParameterView = new ProductParameterView();
            productParameterView.ShowDialog();
            viewModel = IoC.GetViewModel<TaskOrderViewModel>(this);
            systemLogViewModel = IoC.Get<ISystemLogViewModel>();
            this.DataContext = viewModel;
            viewModel.LoadAllInfos();

            datagrid1.PreviewMouseDoubleClick += Datagrid1_PreviewMouseDoubleClick;
            #region  启动任务
            CommandBindings.Add(new CommandBinding(MyCommands.Start,
                (s, e) =>
                {
                    MessageBoxResult result;
                    //TaskOrder runningOrder= materialOrderManager.RunningOrderCheck(viewModel.SelectedModel);
                    //if(runningOrder !=null)
                    //{
                    //    result = MessageBoxX.Show($"该工段存在其他正在运行的任务，确定直接在线切换至{viewModel.SelectedModel.order_number}吗？", "确认", Application.Current.MainWindow, MessageBoxButton.YesNo);
                    //}
                    //else
                    //{
                    //    result = MessageBoxX.Show($"确定启动任务{viewModel.SelectedModel.order_number}吗？", "确认", Application.Current.MainWindow, MessageBoxButton.YesNo);
                    //}
                    
                    //if (result == MessageBoxResult.Yes)
                    //{
                    //    string errMsg = materialOrderManager.StartRoute(viewModel.SelectedModel);
                    //    if (!string.IsNullOrEmpty(errMsg))
                    //    {
                    //        MessageBoxX.Show(errMsg, "路径启动失败", Application.Current.MainWindow);
                    //    }
                    //    else
                    //    {
                    //        systemLogViewModel.AddMewStatus($"启动原料计划{viewModel.SelectedModel.line_code}{viewModel.SelectedModel.material_name}");
                    //        if (runningOrder !=null)
                    //        {
                    //            systemLogViewModel.AddMewStatus($"在线结束原料计划{runningOrder.line_code}{runningOrder.material_name}");
                    //            if (materialOrderManager.FinishOrder(runningOrder, TaskCommandEnum.FinishAndSwitch) == false)
                    //            {
                    //                MessageBoxX.Show("任务完成失败！", "错误", Application.Current.MainWindow);
                    //            }
                    //            else
                    //            {
                    //                viewModel.Remove(runningOrder);
                    //            }
                    //        }
                    //    }
                    //}

                },
                (s, e) =>
                {
                    e.CanExecute = viewModel.SelectedModel != null && viewModel.SelectedModel.order_status == (int)OrderStatusEnum.Created;
                }));
            #endregion
            #region  重启任务
            CommandBindings.Add(new CommandBinding(MyCommands.Restart,
                (s, e) =>
                {
                    //string errMsg = materialOrderManager.StartRoute(viewModel.SelectedModel);
                    //if (!string.IsNullOrEmpty(errMsg))
                    //{
                    //    MessageBoxX.Show(errMsg, "错误", Application.Current.MainWindow);
                    //}
                    //else
                    //{
                    //    systemLogViewModel.AddMewStatus($"重启原料计划{viewModel.SelectedModel.line_code}{viewModel.SelectedModel.material_name}");
                    //}
                },
                (s, e) =>
                {
                    e.CanExecute = viewModel.SelectedModel != null && 
                                   (viewModel.SelectedModel.order_status == (int)OrderStatusEnum.Starting || viewModel.SelectedModel.order_status == (int)OrderStatusEnum.Running);
                }));
            #endregion
            #region  完成任务
            CommandBindings.Add(new CommandBinding(MyCommands.Stop,
                (s, e) =>
                {
                    //MessageBoxResult result = MessageBoxX.Show($"确定完成任务{viewModel.SelectedModel.order_number}吗？", "确认", Application.Current.MainWindow, MessageBoxButton.YesNo);
                    //if (result == MessageBoxResult.Yes)
                    //{
                    //    if (materialOrderManager.FinishOrder(viewModel.SelectedModel, TaskCommandEnum.Stop) == false)
                    //    {
                    //        MessageBoxX.Show("任务完成失败！", "错误", Application.Current.MainWindow);
                    //    }
                    //    else
                    //    {
                    //        systemLogViewModel.AddMewStatus($"完成原料计划{viewModel.SelectedModel.line_code}{viewModel.SelectedModel.material_name}");
                    //        viewModel.Remove(viewModel.SelectedModel);
                    //    }
                    //}
                },
                (s, e) =>
                {
                    e.CanExecute = viewModel.SelectedModel != null &&
                                   (viewModel.SelectedModel.order_status == (int)OrderStatusEnum.Starting || viewModel.SelectedModel.order_status == (int)OrderStatusEnum.Running); ;
                }));
            #endregion
            #region  取消任务
            CommandBindings.Add(new CommandBinding(MyCommands.Cancel,
                (s, e) =>
                {
                    //MessageBoxResult result = MessageBoxX.Show($"确定取消任务{viewModel.SelectedModel.order_number}吗？", "确认", Application.Current.MainWindow, MessageBoxButton.YesNo);
                    //if (result == MessageBoxResult.Yes)
                    //{
                    //    if (materialOrderManager.FinishOrder(viewModel.SelectedModel, TaskCommandEnum.EstopAndCancel) == false)
                    //    {
                    //        MessageBoxX.Show("任务取消失败！", "错误", Application.Current.MainWindow);
                    //    }
                    //    else
                    //    {
                    //        systemLogViewModel.AddMewStatus($"取消原料计划{viewModel.SelectedModel.line_code}{viewModel.SelectedModel.material_name}");
                    //        viewModel.Remove(viewModel.SelectedModel);
                    //    }
                    //}
                },
                (s, e) =>
                {
                    e.CanExecute = viewModel.SelectedModel != null &&
                                  (viewModel.SelectedModel.order_status == (int)OrderStatusEnum.Starting || viewModel.SelectedModel.order_status == (int)OrderStatusEnum.Running); ;
                }));
            #endregion
            #region  删除任务
            CommandBindings.Add(new CommandBinding(MyCommands.Delete,
                (s, e) =>
                {
                    ContextMenu_Click("Delete");
                    systemLogViewModel.AddMewStatus($"用户删除原料计划");
                },
                (s, e) =>
                {
                    e.CanExecute = e.CanExecute = viewModel.ModelList.Where(x => x.order_status != (int)OrderStatusEnum.Running).ToList().Count > 0;
                }));
            #endregion
        }

        private void Datagrid1_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void datagrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(datagrid1.SelectedItem !=null)
            {
                viewModel.SelectedModel = datagrid1.SelectedItem as TaskOrder;
                //if(viewModel.SelectedModel.order_status!= (int)OrderStatusEnum.Created)
                //{
                //    DataGridVisualHelper.SetEnableRowsMove(datagrid1, false);
                //}
                //else
                //{
                //    DataGridVisualHelper.SetEnableRowsMove(datagrid1, true);
                //}
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
        }
        //编辑窗体关闭刷新
        private void EditView_Closed(object sender, EventArgs e)
        {
            this.DataContext = null;
            viewModel.LoadAllInfos();
            this.DataContext = viewModel;
        }


        public void ContextMenu_Click(string click_name)
        {
            if (click_name == "New")//新建
            {
                viewModel.EditModel = new TaskOrder();
                //TaskOrderEditView editView = new TaskOrderEditView(viewModel);
                //editView.Topmost = true;
                //editView.Show();
                //editView.Closed += EditView_Closed;
            }
            else if (click_name == "Edit")//编辑
            {
                if (datagrid1.SelectedItem as TaskOrder != null)
                {
                    viewModel.EditModel = datagrid1.SelectedItem as TaskOrder;//当前编辑对象
                    if (viewModel.EditModel.order_status == (int)OrderStatusEnum.Created)
                    {
                        //TaskOrderEditView editView = new TaskOrderEditView(viewModel);
                        //editView.Topmost = true;
                        //editView.Show();
                        //editView.Closed += EditView_Closed;
                    }
                    else
                    {
                        MessageBoxX.Show("任务单仅在未执行前进行编辑！", "错误", Application.Current.MainWindow);
                    }
                }
            }
            else if (click_name == "Delete")//删除
            {
                var list = viewModel.ModelList.Where(x => x.order_status != (int)OrderStatusEnum.Running).ToList();
                if (list.Count > 0)
                {
                    MessageBoxResult result = MessageBoxX.Show($"确定删除任务吗？", "确认", Application.Current.MainWindow, MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        foreach (var item in list)
                        {
                            if (viewModel.Remove(item) == false)
                            {
                                MessageBoxX.Show("任务删除失败！", "错误", Application.Current.MainWindow);
                            }
                        }
                    }
                }
            }
        }
    }
}
