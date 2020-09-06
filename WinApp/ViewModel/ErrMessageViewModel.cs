using GalaSoft.MvvmLight;
using iFactory.DevComServer;

namespace iFactoryApp.ViewModel
{
    /// <summary>
    ///
    /// </summary>
    public class ErrMessageViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public ErrMessageViewModel()
        {
           
        }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string MessageContent { set; get; }
        /// <summary>
        /// 复位的标签
        /// </summary>
        public Tag<short>RstTag { set; get; }
        /// <summary>
        /// 复位的值
        /// </summary>
        public int RstValue { set; get; }
        /// <summary>
        /// 是否自动确认，自动确认则自动关闭
        /// </summary>
        public bool IsAutoAck { set; get; } = true;
    }
}