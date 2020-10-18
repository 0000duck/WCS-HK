using iFactoryApp.ViewModel;
using System;
using System.Windows;
using System.Windows.Threading;

namespace iFactoryApp.View
{
    /// <summary>
    /// MessageBoxView.xaml 的交互逻辑
    /// </summary>
    public partial class ErrMessageView : Window
    {
        private readonly ErrMessageViewModel viewModel;
        private readonly DispatcherTimer timer;

        public ErrMessageView(ErrMessageViewModel errMessageViewModel)
        {
            viewModel = errMessageViewModel;
            InitializeComponent();
            this.DataContext = viewModel;
            if(viewModel.IsAutoAck)
            {
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(5);
                timer.IsEnabled = true;
                timer.Tick += Timer_Tick;
                timer.Start();
            }
        }
        /// <summary>
        /// 自动确认时间到达
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            button_Click(null,null);
        }
        /// <summary>
        /// 确认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.RstTag != null)
            {
                viewModel.RstTag.Write((short)viewModel.RstValue);
            }
            this.Close();
        }
    }
}
