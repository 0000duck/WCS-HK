using iFactory.CommonLibrary;
using iFactory.CommonLibrary.Interface;
using iFactory.DataService.IService;
using iFactory.DevComServer;
using iFactoryApp.Common;
using iFactoryApp.View;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace iFactoryApp
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            GlobalSettings.LoadSettings();//加载参数
            IoC.SetupIoC();//IoC容器启用
            var log = IoC.Get<ILogWrite>();
            var service1 = IoC.Get<IDatabaseTagGroupService>();
            var service2 = IoC.Get<IDatabaseTagService>();
            PLCManager.SetService(service1, service2, log);
          
            log.WriteLog("程序启动");

            UserViewService.LoadLoginWindow();
            Application.Current.Exit += Current_Exit;
        }
       
        private static System.Threading.Mutex mutex;
        protected override void OnStartup(StartupEventArgs e)
        {
            mutex = new System.Threading.Mutex(true, "OnlyRun_CRNS");
            if (mutex.WaitOne(0, false))
            {
                base.OnStartup(e);
            }
            else
            {
                MessageBox.Show("程序已经在运行！", "提示");
                this.Shutdown();
            }
        }
        /// <summary>
        /// 程序退出清理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Current_Exit(object sender, ExitEventArgs e)
        {
            IoC.Dispose();
            Environment.Exit(0);
        }
    }
}
