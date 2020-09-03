using iFactory.CommonLibrary;
using iFactory.UserManage;
using iFactoryApp.ViewModel;
using Panuon.UI.Silver;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Unity;

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
            WriteRFIDWindow = new RFIDLib.RFIDWindow("COM1",false);//写入端口号
            ReadRFIDWindow = new RFIDLib.RFIDWindow("COM2");//读取端口号
            ReadRFIDWindow.button_read_successive_Click(null,null);//连续读取
            fram1.NavigateToPage(WriteRFIDWindow, false);
            fram2.NavigateToPage(ReadRFIDWindow, false);
        }

        public void ContextMenu_Click(string click_name)
        {
            MessageBoxX.Show("当前窗体此功能不可用", "警告", Application.Current.MainWindow, MessageBoxButton.OK);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;  // cancels the window close    
            this.Hide();      // Programmatically hides the window
        }
    }
}
