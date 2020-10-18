using iFactory.CommonLibrary;
using iFactory.DataService.IService;
using iFactory.DataService.Model;
using System.Windows;

namespace iFactory.UserManage
{
    /// <summary>
    /// UserUpdateWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UserUpdatePwdWindow : System.Windows.Window
    {
        private UserManageViewModel viewModel;
        public UserUpdatePwdWindow(UserManageViewModel userManageViewModel)
        {
            InitializeComponent();
            viewModel = userManageViewModel;
            viewModel.EditModel = GloableUserInfo.LoginUser;
            this.DataContext = viewModel;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            string _old = txt_Old.Text.Trim();
            string _new = txt_New.Text.Trim();

            if (string.IsNullOrEmpty(_old) || string.IsNullOrEmpty(_new))
            {
                MyFormExtensionMethods.MessageBoxShow("内容不能为空!", "错误");
                return;
            }
            var user = viewModel.userService.QueryableToEntity(x=>x.id== viewModel.EditModel.id);
            if (user.user_password != _old)
            {
                MyFormExtensionMethods.MessageBoxShow("原密码错误!", "错误");
                return;
            }
            else
            {
                viewModel.EditModel.user_password = _new;
                bool re = viewModel.userService.UpdateEntity(viewModel.EditModel);
                if (re)
                {
                    MyFormExtensionMethods.MessageBoxShow("修改成功!", "提示");
                    this.Close();
                }
                else
                {
                    MyFormExtensionMethods.MessageBoxShow("修改失败!", "错误");
                }
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
