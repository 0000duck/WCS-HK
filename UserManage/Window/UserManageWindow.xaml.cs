using iFactory.CommonLibrary;
using iFactory.DataService.IService;
using iFactory.DataService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace iFactory.UserManage
{
    /// <summary>
    /// UserUpdateWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UserManageWindow : System.Windows.Window
    {
        private NodeData SelectedNode;
        private ObservableCollection<NodeData> roots = new ObservableCollection<NodeData>();
        private readonly UserManageViewModel viewModel;

        public UserManageWindow(UserManageViewModel userManageViewModel)
        {
            InitializeComponent();
            viewModel = userManageViewModel;
            this.DataContext = viewModel;
            treeView1.IsEnabled = true;
            #region  新建用户
            CommandBindings.Add(new CommandBinding(
                ApplicationCommands.New,
                (s, e) =>
                {
                    viewModel.EditModel = new SystemUser();
                    UserCreateWindow win = new UserCreateWindow(userManageViewModel);
                    win.Topmost = true;
                    win.Show();
                },
                (s, e) =>
                {
                    e.CanExecute = SelectedNode!=null && GloableUserInfo.LoginUser.user_type >= (int)UserType.Manager;
                }));
            #endregion
            #region  编辑用户 修改信息
            CommandBindings.Add(new CommandBinding(
                ApplicationCommands.Properties,
                (s, e) =>
                {
                    try
                    {
                        viewModel.EditModel = viewModel.SelectModel;
                        UserCreateWindow win = new UserCreateWindow(userManageViewModel);
                        win.Show();
                    }
                    catch (Exception ex)
                    {

                        MessageBox.Show("数据保存失败！");
                    }
                },
                (s, e) =>
                {
                    e.CanExecute = viewModel.SelectModel != null && GloableUserInfo.LoginUser.user_type >= (int)UserType.Manager;
                }));
            #endregion
            #region  删除用户
            CommandBindings.Add(new CommandBinding(
                ApplicationCommands.Delete,
                (s, e) =>
                {
                    if (System.Windows.MessageBox.Show("确认删除该用户？", "警告", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        viewModel.Remove(viewModel.SelectModel);
                        viewModel.SelectModel = null;
                    }
                },
                (s, e) =>
                {
                    e.CanExecute = viewModel.SelectModel != null && GloableUserInfo.LoginUser.user_type >= (int)UserType.Manager; 
                }));
            #endregion
            
        }
        void ShowTreeView()
        {
            var list=EnumHelper.ConvertEnumToList<int>(typeof(UserType));
            //var list = dc.system_user.GroupBy(x => x.user_type).ToList();
            foreach (var item in list)
            {
                NodeData n = new NodeData();
                n.Name = item.Description;//device_id
                n.Id = item.Value;//device_id
                roots.Add(n);
            }
            treeView1.ItemsSource = roots;
        }

        private void treeView1_Loaded(object sender, RoutedEventArgs e)
        {
            ShowTreeView();
        }
        private void treeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectedNode = treeView1.SelectedItem as NodeData;
          
            if (SelectedNode != null)
            {
                viewModel.LoadAllModels(SelectedNode.Id);
            }
        }

        private void DataGrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid datagrid = sender as DataGrid;
            if(datagrid.SelectedIndex>0)
            {
                viewModel.SelectModel = datagrid.SelectedItem as SystemUser;
            }
            else
            {
                viewModel.SelectModel = null;
            }
        }
    }

    public class NodeData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Root { get; set; }
        public string DisplayName { get; set; }

        public List<NodeData> ChildNodes { get; set; }

        public NodeData()
        {
            ChildNodes = new List<NodeData>();
        }
    }
    /// <summary>
    /// 获取用户类别转换器
    /// </summary>
    public class GetTypeNameConverter : IValueConverter
    {
        /// <summary>
        /// 将数据转换成需要显示的格式  
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int keyValue =System.Convert.ToInt32(value);
            var item= GloableUserInfo.UserTypeList.FirstOrDefault(x => x.Value == keyValue);
            if (item != null)
            {
                return item.Description;
            }
            return null;
        }
        /// <summary>
        /// 将显示的数据格式转回  
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns> 
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string des = value.ToString();
            var item = GloableUserInfo.UserTypeList.FirstOrDefault(x => x.Description == des);
            if (item != null)
            {
                return item.Value;
            }
            return 0;
        }
    }
}
