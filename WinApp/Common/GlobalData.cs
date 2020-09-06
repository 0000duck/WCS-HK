using iFactory.DevComServer;
using System.Collections.Generic;
using System.Configuration;
using System.IO.Ports;

namespace iFactoryApp.Common
{
    /// <summary>
    /// 全局参数设置与配置
    /// </summary>
    public class GlobalData
    {
        /// <summary>
        /// 相机参数
        /// </summary>
        public static List<TcpParmeters> CameraConfig = new List<TcpParmeters>();
        /// <summary>
        /// 机械手参数
        /// </summary>
        public static List<TcpParmeters> RobotConfig = new List<TcpParmeters>();
        /// <summary>
        /// 重量传感器串口参数
        /// </summary>
        public static SerialConfig WeightConfig = new SerialConfig();
        /// <summary>
        /// 全局消息事件
        /// </summary>
        public static ErrorMessageEventClass ErrMsgObject = new ErrorMessageEventClass();

        public GlobalData()
        { }

        public static void LoadSettings()
        {
            for (int i = 1; i <= 2; i++)
            {
                TcpParmeters tcpParmeters = new TcpParmeters();
                tcpParmeters.Ip = ConfigurationManager.AppSettings[$"Port{i}_Ip"];
                tcpParmeters.Port = int.Parse(ConfigurationManager.AppSettings[$"Port{i}_Number"]);
                tcpParmeters.DecimalNum = int.Parse(ConfigurationManager.AppSettings[$"Port{i}_DecimalNum"]);
                tcpParmeters.Ratio = decimal.Parse(ConfigurationManager.AppSettings[$"Port{i}_Ratio"]);
                CameraConfig.Add(tcpParmeters);
            }

            for (int i = 3; i <= 5; i++)
            {
                TcpParmeters tcpParmeters = new TcpParmeters();
                tcpParmeters.Ip = ConfigurationManager.AppSettings[$"Port{i}_Ip"];
                tcpParmeters.Port = int.Parse(ConfigurationManager.AppSettings[$"Port{i}_Number"]);
                tcpParmeters.DecimalNum = int.Parse(ConfigurationManager.AppSettings[$"Port{i}_DecimalNum"]);
                tcpParmeters.Ratio = decimal.Parse(ConfigurationManager.AppSettings[$"Port{i}_Ratio"]);
                RobotConfig.Add(tcpParmeters);
            }

        }

        /// <summary>
        /// 加载串口设置
        /// </summary>
        /// <param name="config"></param>
        private static void LoadSerialConfig(SerialConfig config)
        {
            string tmp;
            config.PortName = ConfigurationManager.AppSettings["PortName"];
            tmp = ConfigurationManager.AppSettings["PortParity"];
            switch (tmp)             //校验位
            {
                case "None":
                    WeightConfig.PortParity = Parity.None;
                    break;
                case "Odd":
                    WeightConfig.PortParity = Parity.Odd;
                    break;
                case "Even":
                    WeightConfig.PortParity = Parity.Even;
                    break;
                default:
                    WeightConfig.PortParity = Parity.None;
                    break;
            }
            tmp = ConfigurationManager.AppSettings["PortStopBits"];
            switch (tmp)            //停止位
            {
                case "1":
                    config.PortStopBits = StopBits.One;
                    break;
                case "1.5":
                    config.PortStopBits = StopBits.OnePointFive;
                    break;
                case "2":
                    config.PortStopBits = StopBits.Two;
                    break;
                default:
                    config.PortStopBits = StopBits.One;
                    break;
            }
            config.PortDataBits = int.Parse(ConfigurationManager.AppSettings["PortDataBits"]);
            config.PortBaudRate = int.Parse(ConfigurationManager.AppSettings["PortBaudRate"]);
            config.Ratio = decimal.Parse(ConfigurationManager.AppSettings["Ratio"]);
            config.ReadCommand = ConfigurationManager.AppSettings["ReadCommand"];
            config.DataFormat = SerialDataFormatEnum.Ascii;
            config.DecimalNum = int.Parse(ConfigurationManager.AppSettings["DecimalNum"]);
        }

    }
}
