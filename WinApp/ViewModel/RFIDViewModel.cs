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
            WriteRFIDWindow = new RFIDLib.RFIDWindow("COM1", false);//写入端口号
            ReadRFIDWindow = new RFIDLib.RFIDWindow("COM2");//读取端口号
            ReadRFIDWindow.button_read_successive_Click(null, null);//连续读取
        }
    }
}