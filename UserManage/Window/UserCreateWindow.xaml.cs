using iFactory.CommonLibrary;
using iFactory.DataService.IService;
using iFactory.DataService.Model;
using System;
using System.Collections.Generic;
using System.Windows;

namespace iFactory.UserManage
{
    /// <summary>
    /// UserUpdateWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UserCreateWindow : System.Windows.Window
    {
        private UserManageViewModel viewModel;
        public bool CancelBit = true;

        public UserCreateWindow(UserManageViewModel userManageViewModel)
        {
            InitializeComponent();
            viewModel = userManageViewModel;
            this.DataContext = viewModel;
            foreach(var item in viewModel.TypeList)//直接给值不行
            {
                cmb.Items.Add(item);
            }
        }
        
        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (txtUserName.Text.Length == 0)
            {
                MyFormExtensionMethods.MessageBoxShow("用户名称长度为0!", "错误");
                return;
            }
            if (txtPassword.Text.Length == 0)
            {
                MyFormExtensionMethods.MessageBoxShow("密码长度为0!", "错误");
                return;
            }
            if (viewModel.EditModel.user_type > GloableUserInfo.LoginUser.user_type)
            {
                MyFormExtensionMethods.MessageBoxShow("用户的级别大于当前登录用户等级，不能添加!", "错误");
                return;
            }
            if (viewModel.EditModel.id==0)//新建模式
            {
                if (viewModel.userService.QueryableToEntity(x=>x.user_name==txtUserName.Text) !=null)
                {
                    MyFormExtensionMethods.MessageBoxShow("该用户名已存在!", "错误");
                    return;
                }
                if (viewModel.userService.Insert(viewModel.EditModel))
                {
                    MyFormExtensionMethods.MessageBoxShow("添加成功!", "提示");
                    viewModel.ModelList.Add(viewModel.EditModel);
                    this.Close();
                }
                else
                {
                    MyFormExtensionMethods.MessageBoxShow("添加失败!", "错误");
                }
            }
            else
            {
                if (viewModel.userService.Update(viewModel.EditModel))
                {
                    MyFormExtensionMethods.MessageBoxShow("修改成功!");
                    CancelBit = false;
                    this.Close();
                }
                else
                {
                    MyFormExtensionMethods.MessageBoxShow("修改失败!");
                }
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
