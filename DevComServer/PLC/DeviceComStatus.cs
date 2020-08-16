using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFactory.DevComServer
{
    /// <summary>
    /// 设备连接状态
    /// </summary>
    public class DeviceComStatus: INotifyPropertyChanged
    {
        public DeviceComStatus()
        { }

        private bool device1Status = false;
        /// <summary>
        /// 设备1连接状态
        /// </summary>
        public bool Device1Status
        {
            set 
            {
                device1Status = value;
                NotifyPropertyChanged("Device1Status");
            }
            get => device1Status;
        }


        private bool device2Status = false;
        /// <summary>
        /// 设备2连接状态
        /// </summary>
        public bool Device2Status
        {
            set
            {
                device2Status = value;
                NotifyPropertyChanged("Device2Status");
            }
            get => device2Status;
        }

        private bool device3Status = false;
        /// <summary>
        /// 设备3连接状态
        /// </summary>
        public bool Device3Status
        {
            set
            {
                device3Status = value;
                NotifyPropertyChanged("Device3Status");
            }
            get => device3Status;
        }

        private bool device4Status = false;
        /// <summary>
        /// 设备4连接状态
        /// </summary>
        public bool Device4Status
        {
            set
            {
                device4Status = value;
                NotifyPropertyChanged("Device4Status");
            }
            get => device4Status;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
