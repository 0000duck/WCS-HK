using iFactory.CommonLibrary;
using iFactoryApp.ViewModel;
using System.ComponentModel;
using System.Windows;

namespace iFactoryApp.View
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RFIDView : Window
    {
        private readonly RFIDViewModel viewModel;

        public RFIDView()
        {
            InitializeComponent();
            viewModel = IoC.GetViewModel<RFIDViewModel>(this);
            this.DataContext = viewModel;

            fram1.NavigateToPage(viewModel.WriteRFIDWindow, false);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;  // cancels the window close    
            this.Hide();      // Programmatically hides the window
        }
    }
}
