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
            this.DataContext = viewModel;
        }

        private void Datagrid1_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void datagrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }
        private void ContextMenu_Click(object sender, RoutedEventArgs e)
        {
            MenuItem contextMenu = e.Source as MenuItem;
            if (contextMenu.Name == "New")//新建
            {
                ContextMenu_Click(contextMenu.Name);
            }
            else if (contextMenu.Name == "Edit")//编辑
            {
                ContextMenu_Click(contextMenu.Name);
            }
            else if (contextMenu.Name == "Delete")//删除
            {
                ContextMenu_Click(contextMenu.Name);
            }
        }
        //编辑窗体关闭刷新
        private void EditView_Closed(object sender, EventArgs e)
        {
            this.DataContext = null;
            viewModel.LoadAllInfos();
            this.DataContext = viewModel;
        }


        public void ContextMenu_Click(string click_name)
        {
            if (click_name == "New")//新建
            {
                viewModel.EditModel = new TaskOrder();
                //TaskOrderEditView editView = new TaskOrderEditView(viewModel);
                //editView.Topmost = true;
                //editView.Show();
                //editView.Closed += EditView_Closed;
            }
            else if (click_name == "Edit")//编辑
            {
               
            }
            else if (click_name == "Delete")//删除
            {
                
            }
        }
    }
}
