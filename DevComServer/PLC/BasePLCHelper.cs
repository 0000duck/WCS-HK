using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFactory.DevComServer
{
    public abstract class BasePLCHelper
    {
        /// <summary>
        /// 地址
        /// </summary>
        public string PLCAddr { set; get; }
        /// <summary>
        /// 端口号
        /// </summary>
        public int PLCPort { set; get; }
        private object obj = new object();
        private bool connectStatus = false;
        /// <summary>
        /// 连接状态
        /// </summary>
        public bool ConnectStatus
        {
            get => connectStatus;
            set => connectStatus = value;
        }
    }
}
