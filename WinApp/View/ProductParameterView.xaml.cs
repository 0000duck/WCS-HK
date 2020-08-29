using iFactory.CommonLibrary;
using iFactory.DataService.Model;
using iFactory.DevComServer;
using iFactoryApp.ViewModel;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace iFactoryApp.View
{
    /// <summary>
    /// LineConfigView.xaml 的交互逻辑
    /// </summary>
    public partial class ProductParameterView : Window//, IContextMenuView
    {
        public readonly ProductParameterViewModel viewModel;
        private readonly ISystemLogViewModel systemLogViewModel;

        public ProductParameterView()
        {
            InitializeComponent();
            viewModel = IoC.GetViewModel<ProductParameterViewModel>(this);
            systemLogViewModel = IoC.Get<ISystemLogViewModel>();
            viewModel.LoadAllInfos();
            this.DataContext = viewModel;
            LoadNewParameters();
            #region 新建
            CommandBindings.Add(new CommandBinding(MyCommands.Add,
               (s, e) =>
               {
                   viewModel.EditModel = new ProductParameter();
                   LoadNewParameters();
                },
               (s, e) =>
               {
                   e.CanExecute = true;
               }));
            #endregion
            #region 保存
            CommandBindings.Add(new CommandBinding(MyCommands.Save,
               (s, e) =>
               {
                   MessageBoxResult result = MessageBoxX.Show($"确定保存该参数吗？", "确认", Application.Current.MainWindow, MessageBoxButton.YesNo);
                   if (result == MessageBoxResult.Yes)
                   {
                       if(string.IsNullOrEmpty(viewModel.EditModel.product_name) || string.IsNullOrEmpty(viewModel.EditModel.graphic_carton_size) ||
                          string.IsNullOrEmpty(viewModel.EditModel.noraml_carton_size) || string.IsNullOrEmpty(viewModel.EditModel.outer_carton_size))
                       {
                           MessageBoxX.Show($"请输入完整参数", "错误", Application.Current.MainWindow, MessageBoxButton.OK);
                           return;
                       }
                       viewModel.Update(viewModel.EditModel);
                   }
               },
               (s, e) =>
               {
                   e.CanExecute = viewModel.EditModel !=null;
               }));
            #endregion
            #region 删除
            CommandBindings.Add(new CommandBinding(MyCommands.Delete,
               (s, e) =>
               {
                   MessageBoxResult result = MessageBoxX.Show($"确定删除该参数吗？", "确认", Application.Current.MainWindow, MessageBoxButton.YesNo);
                   if(result == MessageBoxResult.Yes)
                   {
                       TreeNodeData node = treeview1.SelectedItem as TreeNodeData;
                       if(node !=null)
                       {
                           viewModel.Remove(node.id);
                       }
                   }
               },
               (s, e) =>
               {
                   e.CanExecute = viewModel.EditModel != null && treeview1.SelectedItem !=null;
               }));
            #endregion
        }

        private void treeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeNodeData nodeData = treeview1.SelectedItem as TreeNodeData;
            if (nodeData != null)
            {
                viewModel.EditModel = viewModel.ModelList.FirstOrDefault(x => x.id == nodeData.id);
                LoadNewParameters();
            }
        }
        private void LoadNewParameters()
        {
            group1.DataContext = viewModel.EditModel;
        }
        private void ContextMenu_Click(object sender, RoutedEventArgs e)
        {
            MenuItem contextMenu = e.Source as MenuItem;
            if (contextMenu.Name == "New")//新建
            {
                viewModel.EditModel = new TaskOrder();
            }
            else if (contextMenu.Name == "Save")//编辑
            {
                if (string.IsNullOrEmpty(viewModel.EditModel.product_name))
                {
                    MessageBoxX.Show($"请输入产品名称", "警告", Application.Current.MainWindow, MessageBoxButton.OK);
                    return;
                }
                if(viewModel.Update(viewModel.EditModel)==false)
                {
                    MessageBoxX.Show($"保存参数失败", "警告", Application.Current.MainWindow, MessageBoxButton.OK);
                }
            }
            else if (contextMenu.Name == "Delete")//删除
            {
                if(viewModel.EditModel !=null)
                {
                    var res= MessageBoxX.Show($"确定删除参数{viewModel.EditModel.product_name}吗？", "警告", Application.Current.MainWindow, MessageBoxButton.YesNo);
                    if(res== MessageBoxResult.Yes)
                    {
                        viewModel.Remove(viewModel.EditModel);
                    }
                }
            }
        }
        
    }
}
