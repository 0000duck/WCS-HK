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
    public partial class RFIDView : Window
    {
        private readonly RFIDViewModel viewModel;
        public RFIDLib.RFIDWindow WriteRFIDWindow { set; get; }
        public RFIDLib.RFIDWindow ReadRFIDWindow { set; get; }

        public RFIDView()
        {
            InitializeComponent();
            viewModel = IoC.GetViewModel<RFIDViewModel>(this);
            this.DataContext = viewModel;
            WriteRFIDWindow = new RFIDLib.RFIDWindow();
            ReadRFIDWindow = new RFIDLib.RFIDWindow(1);
            ReadRFIDWindow.button_read_successive_Click(null,null);//连续读取
            fram1.NavigateToPage(WriteRFIDWindow, false);
            fram2.NavigateToPage(ReadRFIDWindow, false);
        }

        public void ContextMenu_Click(string click_name)
        {
            MessageBoxX.Show("当前窗体此功能不可用", "警告", Application.Current.MainWindow, MessageBoxButton.OK);
        }
    }
}
