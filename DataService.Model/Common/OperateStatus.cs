using iFactory.CommonLibrary;
using System;

namespace iFactory.DataService.Model
{
    public class OperateStatus
    {
        /// <summary>
        /// 发生的时间
        /// </summary>
        public DateTime Dt { set; get; }
        /// <summary>
        /// 类别
        /// </summary>
        public int Type { set; get; } = (int)LogTypeEnum.Info;
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { set; get; }

        public OperateStatus(string content)
        {
            this.Dt = DateTime.Now;
            this.Content = content;
        }
        public OperateStatus(string content, LogTypeEnum logTypeEnum)
        {
            this.Dt = DateTime.Now;
            this.Type = (int)logTypeEnum;
            this.Content = content;
        }
        public override string ToString()
        {
            return Dt.ToString() + " " + Content;
        }
    }
}
