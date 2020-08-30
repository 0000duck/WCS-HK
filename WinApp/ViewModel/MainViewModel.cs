using GalaSoft.MvvmLight;
using iFactory.DataService.Model;
using iFactory.DevComServer;
using iFactory.UserManage;

namespace iFactoryApp.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
           
            //TagList.PLCGroups[0].PLCGroupObj.IsConnected
        }
        /// <summary>
        /// 系统登录用户
        /// </summary>
        public SystemUser LoginUser
        {
            get
            {
                return GloableUserInfo.LoginUser;
            }
        }

        /// <summary>
        /// plc连接状态
        /// </summary>
        public PLCDevice Device1
        {
            get 
            { 
                if(TagList.PLCGroups !=null && TagList.PLCGroups.Count>0)
                     return TagList.PLCGroups[0].PlcDevice;
                return null;
            }
        }
        /// <summary>
        /// plc连接状态
        /// </summary>
        public PLCDevice Device2
        {
            get
            {
                if (TagList.PLCGroups != null && TagList.PLCGroups.Count > 0)
                    return TagList.PLCGroups[1].PlcDevice;
                return null;
            }
        }
        /// <summary>
        /// plc连接状态
        /// </summary>
        public PLCDevice Device3
        {
            get
            {
                if (TagList.PLCGroups != null && TagList.PLCGroups.Count > 0)
                    return TagList.PLCGroups[2].PlcDevice;
                return null;
            }
        }
        /// <summary>
        /// plc连接状态
        /// </summary>
        public PLCDevice Device4
        {
            get
            {
                if (TagList.PLCGroups != null && TagList.PLCGroups.Count > 0)
                    return TagList.PLCGroups[3].PlcDevice;
                return null;
            }
        }
    }
}