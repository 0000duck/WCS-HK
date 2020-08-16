using iFactory.DataService.IService;
using iFactory.EncryptLib;
using System.Windows;
using System.Windows.Input;

namespace iFactory.UserManage
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UserLoginWindow : System.Windows.Window
    {
        private readonly IUserService _userService;

        public UserLoginWindow(IUserService userService)
        {
            InitializeComponent();
            _userService = userService;
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtUser.Text))
            {
                txterr.Text = "用户名不正确!不可为空";
                return;
            }
            if (string.IsNullOrEmpty(txtPassword.Password))
            {
                txterr.Text = "用户密码不正确!不可为空";
                return;
            }
            //bool codeBit = SysAuthorization.CheckEncodeInfo();
            //if (codeBit == false)
            //{
            //    MessageBox.Show("授权登录失败");
            //}

            if (_userService.UserLogin(txtUser.Text, txtPassword.Password))
            {
                GloableUserInfo.LoginUser= _userService.QueryableToEntity(x=>x.user_name==txtUser.Text);
                this.Hide();
                if(UserLoginEvent!=null)
                {
                    UserLoginEvent(GloableUserInfo.LoginUser, null);
                }
            }
            else
            {
                System.Windows.MessageBox.Show("用户名或密码错误！请重新输入", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnQuit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        public delegate void UserLoginDelegate(object sender, ExecutedRoutedEventArgs e);
        /// <summary>
        /// 用户登录成功事件
        /// </summary>
        public event UserLoginDelegate UserLoginEvent = delegate { };

    }
}
