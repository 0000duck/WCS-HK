using iFactory.DataService.IService;
using iFactory.UserManage;
//using iFactory.UserManage;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace iFactoryApp.View
{
    public class UserViewService
    {
        /// <summary>
        /// 加载登录页面
        /// </summary>
        public static void LoadLoginWindow()
        {
            GloableUserInfo.LoginUser = null;
            var userService = IoC.Get<IUserService>();
            UserLoginWindow loginWin = new UserLoginWindow(userService);
            loginWin.Show();
            loginWin.UserLoginEvent += Win_UserLoginEvent;
        }
        /// <summary>
        /// 登录成功显示主界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Win_UserLoginEvent(object sender, ExecutedRoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
        /// <summary>
        /// 检查授权
        /// </summary>
        /// <returns></returns>
        public static bool CheckAuthorize(UserType userType)
        {
            if (GloableUserInfo.LoginUser != null)
            {
                if (GloableUserInfo.LoginUser.user_type >= (int)userType)
                {
                    return true;
                }
            }

            MessageBoxX.Show($"用户无权限操作!", "确认", System.Windows.Application.Current.MainWindow, MessageBoxButton.OK);
            return false;
        }
    }
}
