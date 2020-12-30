using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using iFactory.CommonLibrary;
using iFactory.CommonLibrary.Interface;
using iFactory.DataService.Model;
using iFactory.DevComServer;
using iFactoryApp.Common;
using System;
using System.Collections.ObjectModel;

namespace iFactoryApp.ViewModel
{
    public interface ISystemLogViewModel
    {
        void AddMewStatus(string Content, LogTypeEnum logTypeEnum = LogTypeEnum.Info, bool IsRecordToFile = true);
        /// <summary>
        /// 传递错误消息通知并传递复位标签
        /// </summary>
        /// <param name="Content"></param>
        /// <param name="RstTag"></param>
        /// <param name="RstValue"></param>
        void AddNewAutoAckWindowInfo(string Content, Tag<short>RstTag,short RstValue);
        /// <summary>
        /// 传递错误消息通知并传递复位标签
        /// </summary>
        /// <param name="Content"></param>
        /// <param name="RstTag"></param>
        /// <param name="RstValue"></param>
        void AddNewAckWindowInfo(string Content, Tag<short> RstTag, short RstValue);
    }
    public class SystemLogViewModel : ViewModelBase, ISystemLogViewModel
    {
        private readonly ILogWrite _log;

        public SystemLogViewModel(ILogWrite log)
        {
            _log = log;
        }

        private ObservableCollection<OperateStatus> operatesCollection = new ObservableCollection<OperateStatus>();
        /// <summary>
        /// 运行日志
        /// </summary>
        public ObservableCollection<OperateStatus> OperatesCollection
        {
            set { operatesCollection = value; }
            get { return operatesCollection; }
        }

        //private Action closeAction;
        public Action CloseAction { set; get; }

        #region 全局命令
        private RelayCommand logClearCmd;
        /// <summary>
        /// 执行提交命令的方法
        /// </summary>
        public RelayCommand LogClearCmd
        {
            get
            {
                if (logClearCmd == null) return new RelayCommand(() => ExcuteClearCommand(), CanExcute);
                return logClearCmd;
            }
            set { logClearCmd = value; }
        }

        #endregion

        #region 附属方法

        /// <summary>
        /// 执行提交方法
        /// </summary>
        private void ExcuteClearCommand()
        {
            operatesCollection.Clear();
        }
        /// <summary>
        /// 是否可执行（这边用表单是否验证通过来判断命令是否执行）
        /// </summary>
        /// <returns></returns>
        private bool CanExcute()
        {
            return true;
        }
        /// <summary>
        /// 新增状态
        /// </summary>
        /// <param name="Content"></param>
        /// <param name="logTypeEnum"></param>
        /// <param name="IsRecordToFile"></param>
        public void AddMewStatus(string Content, LogTypeEnum logTypeEnum = LogTypeEnum.Info, bool IsRecordToFile = true)
        {
            if (operatesCollection.Count >= 500)
            {
                ObservableCollectionHelper.ClearItem<OperateStatus>(operatesCollection);
            }
            OperateStatus operateStatus = new OperateStatus(Content, logTypeEnum);
            ObservableCollectionHelper.InsertItem<OperateStatus>(operatesCollection, operateStatus);//增加记录
            if (IsRecordToFile)
            {
                _log.WriteLog(Content);
            }
            if(logTypeEnum== LogTypeEnum.Error)
            {
                GlobalData.ErrMsgObject.SendErrorMessage(Content);//错误消息自动弹窗
            }
        }
        /// <summary>
        /// 触发错误消息弹窗确认
        /// </summary>
        /// <param name="Content"></param>
        /// <param name="RstTag"></param>
        /// <param name="RstValue"></param>
        public void AddNewAutoAckWindowInfo(string Content, Tag<short> RstTag, short RstValue)
        {
            if (operatesCollection.Count >= 500)
            {
                ObservableCollectionHelper.ClearItem<OperateStatus>(operatesCollection);
            }
            OperateStatus operateStatus = new OperateStatus(Content, LogTypeEnum.Error);
            ObservableCollectionHelper.InsertItem<OperateStatus>(operatesCollection, operateStatus);//增加记录
            _log.WriteLog(Content);
            ErrMessageViewModel errMessageViewModel = new ErrMessageViewModel() { IsAutoAck = true, MessageContent = Content, RstTag = RstTag, RstValue = RstValue };

            GlobalData.ErrMsgObject.SendErrorMessage(errMessageViewModel);//错误消息自动弹窗

        }

        public void AddNewAckWindowInfo(string Content, Tag<short> RstTag, short RstValue)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
