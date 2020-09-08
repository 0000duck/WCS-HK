using iFactory.CommonLibrary;
using iFactory.UserManage;
using iFactoryApp.Common;
using iFactoryApp.ViewModel;
using Panuon.UI.Silver;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace iFactoryApp.View
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : WindowX
    {
        private readonly MainViewModel viewModel;
        private readonly RFIDView rfidView = new RFIDView();
        private readonly TaskOrderView taskOrderView = new TaskOrderView();
        private readonly ReportView reportView = new ReportView();
        private IContextMenuView lastView;

        public MainWindow()
        {
            InitializeComponent();
            viewModel = IoC.GetViewModel<MainViewModel>(this);
            this.DataContext = viewModel;
            lastView = taskOrderView;
            GlobalData.ErrMsgObject.ErrorMessageEvent += ErrorMessageEvent;//错误信息弹窗
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Control control = e.Source as Control;   
            if (control == null || control.Tag == null) return;
            switch (control.Tag.ToString())
            {
                case "RFID":
                    if(rfidView.Visibility !=Visibility.Visible)
                    {
                        rfidView.Show();
                    }
                    else
                    {
                        rfidView.Activate();
                    }
                    break;
                case "TaskOrder":
                    frame1.NavigateToPage(taskOrderView, false);
                    lastView = taskOrderView;
                    break;
                case "Report":
                    frame1.NavigateToPage(reportView, false);
                    lastView = null;
                    break;
                case "Product":
                    ProductParameterView productParameterView = new ProductParameterView();
                    productParameterView.ShowDialog();
                    break;
                case "SystemLog":
                    SystemLogView systemLogView = new SystemLogView();
                    systemLogView.ShowDialog();
                    break;
                case "SystemInfo":
                    SystemInfoView systemInfoView = new SystemInfoView();
                    systemInfoView.ShowDialog();
                    break;
                case "New":
                case "Edit":
                case "Delete":
                case "Finish":
                case "Download":
                    if (lastView != null)
                    {
                        lastView.ContextMenu_Click(control.Tag.ToString());
                    }
                    break;
            }
        }

        private void ButtonLoginOut_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            UserViewService.LoadLoginWindow();
        }
        /// <summary>
        /// 密码修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonPwdEdit_Click(object sender, RoutedEventArgs e)
        {
            UserManageViewModel service = IoC.Get<UserManageViewModel>();
            UserUpdatePwdWindow userUpdatePwdWindow = new UserUpdatePwdWindow(service);
            userUpdatePwdWindow.Show();
            userUpdatePwdWindow.Topmost = true;
        }
        /// <summary>
        /// 用户管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonUserManage_Click(object sender, RoutedEventArgs e)
        {
            if (UserViewService.CheckAuthorize(UserType.Admin))
            {
                UserManageViewModel service = IoC.Get<UserManageViewModel>();
                UserManageWindow userManageWindow = new UserManageWindow(service);
                userManageWindow.Show();
                userManageWindow.Topmost = true;
            }
        }

        private void Window_ClosingAsync(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var rest = MessageBoxX.Show("确定退出程序吗？", "警告", Application.Current.MainWindow, MessageBoxButton.YesNo);
            if (rest == MessageBoxResult.Yes)
            {
                e.Cancel = false;
                Application.Current.Shutdown();
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void WindowX_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Point point= e.GetPosition(MainMenuGrid);
            if (point.Y > 0) return;
            if(this.WindowState ==WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
            }
            else if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
        }
        /// <summary>
        /// 错误消息弹窗处理
        /// </summary>
        /// <param name="errMessageViewModel"></param>
        private void ErrorMessageEvent(ErrMessageViewModel errMessageViewModel)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                ErrMessageView messageBoxView = new ErrMessageView(errMessageViewModel);
                messageBoxView.Topmost = true;
                messageBoxView.Show();
            }));
        }
    }
}
