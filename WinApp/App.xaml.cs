using iFactory.CommonLibrary.Interface;
using iFactory.DataService.IService;
using iFactory.DevComServer;
using iFactoryApp.Common;
using iFactoryApp.View;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace iFactoryApp
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private ILogWrite log;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            GlobalData.LoadSettings();//加载参数
            IoC.SetupIoC();//IoC容器启用
            log = IoC.Get<ILogWrite>();
            var service1 = IoC.Get<IDatabaseTagGroupService>();
            var service2 = IoC.Get<IDatabaseTagService>();
            PLCManager.SetService(service1, service2, log);
          
            log.WriteLog("程序启动");

            UserViewService.LoadLoginWindow();
            Application.Current.Exit += Current_Exit;
            //UI线程未捕获异常处理事件
            this.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
            //Task线程内未捕获异常处理事件
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            //非UI线程未捕获异常处理事件
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
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

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                e.Handled = true; //把 Handled 属性设为true，表示此异常已处理，程序可以继续运行，不会强制退出      
                log.WriteLog("捕获App_DispatcherUnhandledException未处理异常", e.Exception);
            }
            catch (Exception ex)
            {
                log.WriteLog($"程序发生致命错误，将终止{ ex.Message}");
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception)
            {
                log.WriteLog("捕获CurrentDomain未处理异常", (Exception)e.ExceptionObject);
            }
            else
            {
                log.WriteLog($"程序发生致命错误，将终止{ e.ExceptionObject.ToString()}");
            }
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            //task线程内未处理捕获
            log.WriteLog($"程序线程内发生致命错误，将终止{ e.ToString()}");
        }
    }
}
