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
        /// ��Ϣ����
        /// </summary>
        public string MessageContent { set; get; }
        /// <summary>
        /// ��λ�ı�ǩ
        /// </summary>
        public Tag<short>RstTag { set; get; }
        /// <summary>
        /// ��λ��ֵ
        /// </summary>
        public int RstValue { set; get; }
        /// <summary>
        /// �Ƿ��Զ�ȷ�ϣ��Զ�ȷ�����Զ��ر�
        /// </summary>
        public bool IsAutoAck { set; get; } = true;
    }
}