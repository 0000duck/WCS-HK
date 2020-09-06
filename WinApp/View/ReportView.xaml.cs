using iFactoryApp.ViewModel;
using Microsoft.Reporting.WinForms;
using System;
using System.Windows;
using System.Windows.Controls;

namespace iFactoryApp.View
{
    /// <summary>
    /// ReportView.xaml 的交互逻辑
    /// </summary>
    public partial class ReportView : Page
    {
        private readonly ReportViewModel viewModel;
        public ReportView()
        {
            InitializeComponent();
            viewModel = IoC.Get<ReportViewModel>();
            this.DataContext = viewModel;
            starttime.Value = DateTime.Now.AddDays(-1);
            endtime.Value = DateTime.Now;
            viewModel.PageSettings(rvProcess, @".\Report\Report1.rdlc");
        }
        private void btnQuery_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (starttime.Value == null || endtime.Value == null || starttime.Value > endtime.Value)
            {
                MessageBox.Show("请选择正确的时间！");//应限制输入超过12小时的上班间隔
                return;
            }
            var list = viewModel.GetHistoryData((DateTime)starttime.Value, (DateTime)endtime.Value,cmbProduct.Text);
            ReportDataSource ds = new ReportDataSource("DataSet1", list);
            rvProcess.LocalReport.DataSources.Clear();
            rvProcess.LocalReport.DataSources.Add(ds);
            rvProcess.RefreshReport();
        }

        private void btnDelete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            bool res = false;
            var rest = MessageBox.Show("确定删除数据吗?", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (rest == MessageBoxResult.Yes)
            {
                res = viewModel.DeleteHistoryData((DateTime)starttime.Value, (DateTime)endtime.Value);
                if (res)
                {
                    MessageBox.Show("删除数据成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("删除数据失败！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
