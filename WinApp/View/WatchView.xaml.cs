using iFactory.CommonLibrary;
using iFactory.UserManage;
using iFactoryApp.ViewModel;
using Panuon.UI.Silver;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace iFactoryApp.View
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WatchView : Page, IContextMenuView
    {
        private readonly WatchViewModel viewModel;
      
        public WatchView()
        {
            InitializeComponent();
            viewModel = IoC.GetViewModel<WatchViewModel>(this);
            this.DataContext = viewModel;
        }

        public void ContextMenu_Click(string click_name)
        {
            MessageBoxX.Show("当前窗体此功能不可用", "警告", Application.Current.MainWindow, MessageBoxButton.OK);
        }
    }
}
