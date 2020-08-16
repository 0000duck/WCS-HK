using iFactory.CommonLibrary.Interface;
using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace iFactory.CommonLibrary
{
    /// <summary>
    /// 通用日志类。配置了Info和Error两个类别
    /// 1.配置在AppLog4Net.config文件中，该文件需要设置为输出到目录
    /// 2.在AssemblyInfo.cs文件中配置文件名称：ConfigFile = "AppLog4Net.config"
    /// </summary>
    public class LogHelper:ILogWrite
    {
        public static ILog loginfo = null;
        public static ILog logerror = null;

        public LogHelper()
        {
            FileInfo fileInfo = new FileInfo(@".\Config\AppLog4Net.config");//加载配置
            SetConfig(fileInfo);
            loginfo = log4net.LogManager.GetLogger("logInfo");
            logerror = log4net.LogManager.GetLogger("logError");
        }
        /// <summary>
        /// 加载配置
        /// </summary>
        public void SetConfig()
        {
            log4net.Config.XmlConfigurator.Configure();
        }
        /// <summary>
        /// 加载配置
        /// </summary>
        /// <param name="configFile"></param>
        public void SetConfig(FileInfo configFile)
        {
            log4net.Config.XmlConfigurator.Configure(configFile);
        }
        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="info"></param>
        public void WriteLog(string content)
        {
            if (loginfo.IsInfoEnabled)
            {
                loginfo.Info(content);
            }
        }
        /// <summary>
        /// 写入错误信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="ex"></param>
        public void WriteLog(string content, Exception ex)
        {
            if (logerror.IsErrorEnabled)
            {
                logerror.Error(content, ex);
            }
        }

        public void WriteLog(string source, string subject, string content)
        {
            if (loginfo.IsInfoEnabled)
            {
                loginfo.Info(source + " " + subject + " " + content);
            }
        }
    }
}
