using iFactory.CommonLibrary;
using iFactory.DataService.IService;
using iFactory.UserManage;
using iFactoryApp.ViewModel;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace iFactoryApp.View
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : WindowX
    {
        private readonly MainViewModel viewModel;
        private IContextMenuView lastView=null;

        public MainWindow()
        {
            InitializeComponent();
            viewModel = IoC.GetViewModel<MainViewModel>(this);
            this.DataContext = viewModel;
          
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Control control = e.Source as Control;   
            if (control == null || control.Tag == null) return;
            switch (control.Tag.ToString())
            {
                case "Product":
                    ProductParameterView productParameterView = new ProductParameterView();
                    productParameterView.ShowDialog();
                    break;
                case "SystemLog":
                    SystemLogView systemLogView = new SystemLogView();
                    systemLogView.ShowDialog();
                    lastView = null;
                    break;
                case "SystemInfo":
                    SystemInfoView systemInfoView = new SystemInfoView();
                    systemInfoView.ShowDialog();
                    lastView = null;
                    break;
                case "New":
                case "Edit":
                case "Delete":
                    if (lastView != null)
                    {
                        lastView.ContextMenu_Click(control.Tag.ToString());
                    }
                    break;
            }
        }
        private void LoadExe()
        {
            string path = ConfigurationManager.AppSettings["Wincc_Report"];
            ProcessStartInfo procInfo = new ProcessStartInfo(path);
            procInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(path);
            procInfo.WindowStyle = ProcessWindowStyle.Normal;
            Process.Start(procInfo);
        }
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;
            TabItem tabItem = e.AddedItems[0] as TabItem;
            if (tabItem == null) return;
            Point point = new Point() { X = 0, Y = gridLayout.Height + tabItem.ActualHeight }; ;
            switch(tabItem.Tag)
            {
                case "Formula":
                    //LoadExe(LayoutFormula, point,"");
                    break;
                case "Material":
                    //LoadExe(LayoutMaterial, point,"");
                    break;
                case "Parmeter":
                    //LoadExe(LayoutParmeter, point,"");
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
    }
}
