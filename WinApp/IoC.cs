using CommonServiceLocator;
using iFactory.CommonLibrary;
using iFactory.CommonLibrary.Interface;
using iFactory.DataService.IService;
using iFactory.DataService.Service;
using iFactory.DevComServer;
using iFactory.UserManage;
using iFactoryApp.Service;
using iFactoryApp.ViewModel;
using Prism.Unity;
using SqlSugar;
using System.Configuration;
using System.Windows.Controls;
using Unity;
using Unity.Resolution;

namespace iFactoryApp
{
    public class IoC
    {
        public static UnityContainer Container;
        /// <summary>
        /// 配置 IoC 容器，绑定所有需要的信息准备使用
        /// 注意：必须在应用程序启动后立即调用，以确保可以找到所有服务
        /// </summary>
        public static void SetupIoC()
        {
            // Register Unity as the IOC container
            Container = new UnityContainer();
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocatorAdapter(Container));
            BindViewModels();//对象绑定
        }
        /// <summary>
        /// 绑定所有的实例对象
        /// </summary>
        private static void BindViewModels()
        {
            ConnectionConfig cfg = new ConnectionConfig()
            {
                ConnectionString = ConfigurationManager.AppSettings["connectionString"],
                DbType = DbType.SqlServer,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute,
                IsShardSameThread = true
            };
            Container.RegisterInstance(typeof(ISqlSugarClient), new SqlSugarClient(cfg));//数据库信息

            //--------------------------------------------------------------------------
            //窗体
            Container.RegisterSingleton<MainViewModel>();
            Container.RegisterSingleton<ISystemLogViewModel, SystemLogViewModel>();

            Container.RegisterSingleton<ProductParameterViewModel>();
            Container.RegisterSingleton<TaskOrderViewModel>();
            Container.RegisterSingleton<RFIDViewModel>();
            Container.RegisterSingleton<ReportViewModel>();
            Container.RegisterSingleton<TagsViewModel>();

            //用户权限
            Container.RegisterSingleton<UserManageViewModel>();

            //--------------------------------------------------------------------------
            //公共操作
            Container.RegisterSingleton<PLCScanTask>();//注入PLC扫描
            Container.RegisterType<ILogWrite, LogHelper>();
            Container.RegisterType<TaskOrderManager>();//任务处理
            //--------------------------------------------------------------------------
            //数据库访问
            Container.RegisterType<IUserService, UserService>();
            Container.RegisterType<IUserGroupService, UserGroupService>();
            //PLC数据字典
            Container.RegisterType<IDatabaseTagGroupService, DatabaseTagGroupService>();
            Container.RegisterType<IDatabaseTagService, DatabaseTagService>();

            Container.RegisterType<IProductParameterService, ProductParameterService>();
            Container.RegisterType<ITaskOrderService, TaskOrderService>();
            Container.RegisterType<ITaskOrderHistoryService, TaskOrderHistoryService>();
            Container.RegisterType<ITaskOrderDetailService, TaskOrderDetailService>();
            Container.RegisterType<ITaskOrderDetailHistoryService, TaskOrderDetailHistoryService>();

            CreateTable();
        }
        /// <summary>
        /// 创建数据库表结构
        /// </summary>
        public static void CreateTable()
        {
            var service1 = Get<IUserService>();
            service1.InitTables();

            var service2 = Get<IUserGroupService>();
            service2.InitTables();

            UserInitial.UserDataInitial(service1, service2);

            var service3 = Get<IDatabaseTagGroupService>();
            service3.InitTables();

            var service4 = Get<IDatabaseTagService>();
            service4.InitTables();

            var service5 = Get<IProductParameterService>();
            service5.InitTables();

            var service6 = Get<ITaskOrderService>();
            service6.InitTables();

            var service7 = Get<ITaskOrderHistoryService>();
            service7.InitTables();

            var service8 = Get<ITaskOrderDetailService>();
            service8.InitTables();

            var service9 = Get<ITaskOrderDetailHistoryService>();
            service9.InitTables();

            //var service10 = Get<IBatchingOrderDetailService>();
            //service10.InitTables();

            //var service11 = Get<IBatchingOrderHistoryService>();
            //service11.InitTables();

            //var service12 = Get<IBatchingOrderDetailHistoryService>();
            //service12.InitTables();
        }
        /// <summary>
        /// 获取注入对象
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public static TService Get<TService>()
        {
            return Container.Resolve<TService>();
        }
        /// <summary>
        /// 专门用于窗体对象获取
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="window">当前需要注入的窗体对象</param>
        /// <returns></returns>
        public static TService GetViewModel<TService>(System.Windows.Window window)
        {
            return Container.Resolve<TService>(new ParameterOverride("window", window));
        }
        /// <summary>
        /// 专门用于窗体对象获取
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="window">当前需要注入的page对象</param>
        /// <returns></returns>
        public static TService GetViewModel<TService>(Page PageWin)
        {
            return Container.Resolve<TService>(new ParameterOverride("Page", PageWin));
        }

        public static void Dispose()
        {
            Container.Dispose();
        }
    }
}
