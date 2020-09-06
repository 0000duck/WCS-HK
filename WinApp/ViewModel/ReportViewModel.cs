using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using iFactory.DataService.IService;
using iFactory.DataService.Model;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Controls;

namespace iFactoryApp.ViewModel
{
    public class ReportViewModel : ViewModelBase
    {
        public ObservableCollection<ProductParameter> ParameterList { set; get; } = new ObservableCollection<ProductParameter>();
        private readonly ITaskOrderHistoryService _historyService;
        private readonly IProductParameterService _productParameterService;
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public ReportViewModel(ITaskOrderHistoryService taskOrderHistoryService,
                               IProductParameterService productParameterService)
        {
            _historyService = taskOrderHistoryService;
            _productParameterService = productParameterService;
            LoadParameters();
        }
        /// <summary>
        /// 加载产品参数列表
        /// </summary>
        public void LoadParameters()
        {
            ParameterList.Clear();
            var list = _productParameterService.QueryableToList(x => x.id > 0).OrderBy(x => x.product_name).ToList();
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    ParameterList.Add(item);
                }
            }
        }
        /// <summary>
        /// 根据时间抓取数据
        /// </summary>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <returns></returns>
        public List<TaskOrderHistory> GetHistoryData(DateTime starttime, DateTime endtime,string product_name)
        {
            List<TaskOrderHistory> list = new List<TaskOrderHistory>();
            if (!string.IsNullOrEmpty(product_name))
            {
                list = _historyService.QueryableToList(x => x.insert_time >= starttime && x.insert_time <= endtime && x.product_name== product_name);
            }
            else
            {
                list = _historyService.QueryableToList(x => x.insert_time >= starttime && x.insert_time <= endtime);
            }
            return list;
        }
        /// <summary>
        /// 根据时间抓取数据
        /// </summary>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <returns></returns>
        public bool DeleteHistoryData(DateTime starttime, DateTime endtime)
        {
            bool res = _historyService.Delete(x => x.insert_time >= starttime && x.insert_time <= endtime);
            return res;
        }
        /// <summary>
        /// reportviewer设置
        /// </summary>
        /// <param name="reportViewer"></param>
        public void PageSettings(ReportViewer reportViewer, string ReportPath)
        {
            PageSettings ps = new PageSettings();
            //ps.Landscape = true;
            ps.Landscape = false;
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.LocalReport.ReportPath = ReportPath;
            reportViewer.SetPageSettings(ps);
            reportViewer.Print += new ReportPrintEventHandler(rp_Print);
        }
        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void rp_Print(object sender, ReportPrintEventArgs e)
        {
            ReportViewer reportViewer = sender as ReportViewer;
            PrinterSettings setting = new PrinterSettings();
            setting.PrintRange = PrintRange.SomePages;
            setting.FromPage = reportViewer.CurrentPage;
            setting.ToPage = reportViewer.CurrentPage;
            e.PrinterSettings = setting;
        }
        #region 全局命令
        private RelayCommand<ListViewItem> viewCmd1;
        /// <summary>
        /// 执行提交命令的方法
        /// </summary>
        public RelayCommand<ListViewItem> ViewCmd1
        {
            get
            {
                if (viewCmd1 == null) return new RelayCommand<ListViewItem>(ExcuteViewCommand1, (ListViewItem p) => { return CanExcute(); });
                return viewCmd1;
            }
            set { viewCmd1 = value; }
        }
        private RelayCommand viewCmd2;
        /// <summary>
        /// 执行提交命令的方法
        /// </summary>
        public RelayCommand ViewCmd2
        {
            get
            {
                if (viewCmd2 == null) return new RelayCommand(() => ExcuteViewCommand2(), CanExcute);
                return viewCmd2;
            }
            set { viewCmd2 = value; }
        }
        private RelayCommand viewCmd3;
        /// <summary>
        /// 执行提交命令的方法
        /// </summary>
        public RelayCommand ViewCmd3
        {
            get
            {
                if (viewCmd3 == null) return new RelayCommand(() => ExcuteViewCommand3(), CanExcute);
                return viewCmd3;
            }
            set { viewCmd3 = value; }
        }
        private RelayCommand<CancelEventArgs> viewCmd4;
        /// <summary>
        /// 执行提交命令的方法
        /// </summary>
        public RelayCommand<CancelEventArgs> ViewCmd4
        {
            get
            {
                if (viewCmd4 == null) return new RelayCommand<CancelEventArgs>(ExcuteViewCommand4, (CancelEventArgs p) => { return CanExcute(); });
                return viewCmd4;
            }
            set { viewCmd4 = value; }
        }
        private RelayCommand viewCmd5;
        /// <summary>
        /// 执行提交命令的方法
        /// </summary>
        public RelayCommand ViewCmd5
        {
            get
            {
                if (viewCmd5 == null) return new RelayCommand(() => ExcuteViewCommand5(), CanExcute);
                return viewCmd5;
            }
            set { viewCmd5 = value; }
        }
        private RelayCommand viewCmd6;
        /// <summary>
        /// 执行提交命令的方法
        /// </summary>
        public RelayCommand ViewCmd6
        {
            get
            {
                if (viewCmd6 == null) return new RelayCommand(() => ExcuteViewCommand6(), CanExcute);
                return viewCmd6;
            }
            set { viewCmd6 = value; }
        }
        #endregion

        #region 附属方法

        /// <summary>
        /// 执行提交方法
        /// </summary>
        public void ExcuteViewCommand1(ListViewItem listViewItem)
        {
            
        }
        private void ExcuteViewCommand2()
        {

        }
        private void ExcuteViewCommand3()
        {

        }
        private void ExcuteViewCommand4(System.ComponentModel.CancelEventArgs e)
        {

        }
        private void ExcuteViewCommand5()
        {

        }
        private void ExcuteViewCommand6()
        {

        }
        /// <summary>
        /// 是否可执行（这边用表单是否验证通过来判断命令是否执行）
        /// </summary>
        /// <returns></returns>
        private bool CanExcute()
        {
            return true;
        }
        #endregion
    }
}
