using iFactory.CommonLibrary.Interface;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace iFactory.DevComServer
{
    /// <summary>
    /// 一个plc访问对象
    /// </summary>
    public class PLCDevice : INotifyPropertyChanged
    {
        /// <summary>
        /// PLC访问对象的名称
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// ip
        /// </summary>
        public string Ip { set; get; }
        /// <summary>
        /// 端口号 
        /// </summary>
        public int Port { set; get; }
        /// <summary>
        /// 站位(Modbus使用)
        /// </summary>
        public byte Station { set; get; }
        /// <summary>
        /// 扫描周期ms
        /// </summary>
        public int CycleTime { set; get; } = 500;
        private bool _IsConnected = false;
        public bool IsConnected
        {
            set
            {
                _IsConnected = value;
                NotifyPropertyChanged("IsConnected");
            }
            get => _IsConnected;
        }
        /// <summary>
        /// 心跳位
        /// </summary>
        public string HeartBit { set; get; }
        /// <summary>
        /// plc类型
        /// </summary>
        public PLCType Type { set; get; } = PLCType.Simens1200;
        /// <summary>
        /// 描述信息
        /// </summary>
        public string Description { set; get; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// 标签队列
        /// </summary>
        public List<ITagGroup> TagGroups { set; get; } = new List<ITagGroup>();
        /// <summary>
        /// 加入队列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="plcGroup"></param>
        /// <param name="tagGroup"></param>
        /// <param name="tag"></param>
        public void AddToGroup<T>(Tag<T> tag)
        {
            TagGroup<T> group = null;

            ITagGroup iGroup = this.TagGroups.FirstOrDefault(x => x.DataType == tag.DataType);
            if (iGroup !=null)
            {
                group = iGroup as TagGroup<T>;
                group.AddToSectionList(tag);//加入连续性扫描片段队列
            }
            else
            {
                group = new TagGroup<T>(tag.DataType.ToString(), Type);
                group.AddToSectionList(tag);//加入连续性扫描片段队列
                this.TagGroups.Add(group);
            }
        }
        /// <summary>
        /// 加入扫描队列
        /// </summary>
        /// <param name="logWrite"></param>
        public void StartToScan(ILogWrite logWrite)
        {
            plcScanTask = new PLCScanTask(this, logWrite);
        }
        /// <summary>
        /// 扫描任务
        /// </summary>
        public PLCScanTask plcScanTask { set; get; }
    }
}
