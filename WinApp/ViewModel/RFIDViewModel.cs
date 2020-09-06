using GalaSoft.MvvmLight;

namespace iFactoryApp.ViewModel
{
    /// <summary>
    ///
    /// </summary>
    public class RFIDViewModel : ViewModelBase
    {
        public RFIDLib.RFIDWindow WriteRFIDWindow { set; get; }
        public RFIDLib.RFIDWindow ReadRFIDWindow { set; get; }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public RFIDViewModel()
        {
            WriteRFIDWindow = new RFIDLib.RFIDWindow("COM1", false);//д��˿ں�
            ReadRFIDWindow = new RFIDLib.RFIDWindow("COM2");//��ȡ�˿ں�
            ReadRFIDWindow.button_read_successive_Click(null, null);//������ȡ
        }
    }
}