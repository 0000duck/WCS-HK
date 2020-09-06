using iFactoryApp.ViewModel;

namespace iFactoryApp.Common
{
    /// <summary>
    /// 消息事件类
    /// </summary>
    public class ErrorMessageEventClass
    {
        /// <summary>
        /// 错误确认事件
        /// </summary>
        /// <param name="errMessageViewModel">消息模型</param>
        public delegate void ErrorMessageEventDelegate(ErrMessageViewModel errMessageViewModel);
        /// <summary>
        /// 
        /// </summary>
        public event ErrorMessageEventDelegate ErrorMessageEvent = delegate { };
        /// <summary>
        /// 发送消息事件
        /// </summary>
        /// <param name="errMessageViewModel"></param>
        public void SendErrorMessage(string Content)
        {
            ErrMessageViewModel errMessageViewModel = new ErrMessageViewModel();
            errMessageViewModel.MessageContent = Content;
            if ((this.ErrorMessageEvent != null))
            {
                this.ErrorMessageEvent(errMessageViewModel);
            }
        }
        /// <summary>
        /// 发送消息事件
        /// </summary>
        /// <param name="errMessageViewModel"></param>
        public void SendErrorMessage(ErrMessageViewModel errMessageViewModel)
        {
            if ((this.ErrorMessageEvent != null))
            {
                this.ErrorMessageEvent(errMessageViewModel);
            }
        }
    }
}
