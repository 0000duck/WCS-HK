using iFactory.CommonLibrary;
using iFactory.DevComServer;
using iFactory.UserManage;
using iFactoryApp.Common;
using iFactoryApp.ViewModel;
using Panuon.UI.Silver;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace iFactoryApp.View
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : WindowX
    {
        private readonly MainViewModel viewModel;
        private readonly RFIDView rfidView = new RFIDView();
        private readonly TaskOrderView taskOrderView;
        private readonly ReportView reportView = new ReportView();
        private readonly ISystemLogViewModel _systemLogViewModel;
        private IContextMenuView lastView;
        private Tag<short> runTag;
        public MainWindow()
        {
            InitializeComponent();
            viewModel = IoC.GetViewModel<MainViewModel>(this);
            _systemLogViewModel = IoC.GetViewModel<ISystemLogViewModel>(this);
            this.DataContext = viewModel;
            taskOrderView = new TaskOrderView();
            frame1.NavigateToPage(taskOrderView, false);
            lastView = taskOrderView;
            GlobalData.ErrMsgObject.ErrorMessageEvent += ErrorMessageEvent;//错误信息弹窗
            if (TagList.PLCGroups != null && TagList.PLCGroups.Count > 0)
            {
                if (TagList.PLCGroups[0].PlcDevice.IsConnected == false)
                {
                    _systemLogViewModel.AddMewStatus("PLC连接失败，请检查设置！",LogTypeEnum.Error);
                }
            }
            TagList.GetTag("system_run", out runTag, "FxPLC");
            if(runTag !=null)
            {
                runTag.PropertyChanged += RunTag_PropertyChanged;
                RunTag_PropertyChanged(runTag,null);
            }
        }

        private void RunTag_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Tag<short> tag = sender as Tag<short>;
            this.Dispatcher.Invoke(() =>
            {
                if (tag.TagValue == 1)
                {
                    Menustart.Header = "启动中";
                    Menustop.Header = "停止";
                    //Menustart.IsEnabled = false;
                    //Menustop.IsEnabled = true;
                    Menustart.Foreground = Brushes.LimeGreen;
                    Menustart.FontWeight = FontWeights.Bold;
                    Menustart.FontStyle = FontStyles.Italic;
                    Menustop.Foreground = Brushes.Black;
                    Menustop.FontWeight = FontWeights.Normal;
                    Menustop.FontStyle = FontStyles.Normal;
                }
                else
                {
                    Menustart.Header = "启动";
                    Menustop.Header = "停止中";
                    //Menustart.IsEnabled = true;
                    //Menustop.IsEnabled = false;
                    Menustop.Foreground = Brushes.Red;
                    Menustop.FontWeight = FontWeights.Bold;
                    Menustop.FontStyle = FontStyles.Italic;
                    Menustart.Foreground = Brushes.Black;
                    Menustart.FontWeight = FontWeights.Normal;
                    Menustart.FontStyle = FontStyles.Normal;
                }
            });
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Control control = e.Source as Control;   
            if (control == null || control.Tag == null) return;
            switch (control.Tag.ToString())
            {
                case "RFID":
                    if (rfidView.Visibility !=Visibility.Visible)
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
                case "TagsView":
                    TagsView tagsView = new TagsView();
                    tagsView.ShowDialog();
                    break;
                case "Start":
                    runTag.Write(1);
                    break;
                case "Stop":
                    runTag.Write(0);
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
