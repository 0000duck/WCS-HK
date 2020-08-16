using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFactory.CommonLibrary.Interface
{
    /// <summary>
    /// 日志写入接口
    /// </summary>
    public interface ILogWrite
    {
        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="content">日志内容</param>
        /// <param name="ex">错误ex</param>
        void WriteLog(string content, Exception ex);
        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="content">日志内容</param>
        void WriteLog(string content);
        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="subject">主题</param>
        /// <param name="content">日志内容</param>
        void WriteLog(string source, string subject, string content);
    }
}
