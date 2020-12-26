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
            WriteRFIDWindow = new RFIDLib.RFIDWindow("COM3", Readmode: false);//д��˿ں�
            ReadRFIDWindow  = new RFIDLib.RFIDWindow("COM4", Readmode: false);//��ȡ�˿ں�
        }
    }
}