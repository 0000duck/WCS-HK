using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace iFactory.DevComServer
{
    public interface ITagGroup
    {
        /// <summary>
        /// 标签组名称
        /// </summary>
        string GroupName { set; get; }
        /// <summary>
        /// 数据类型
        /// </summary>
        TagDataType DataType { set; get; }

    }
    /// <summary>
    ///  变量组对象，包含队列，以及是否激活属性
    /// </summary>
    public class TagGroup<T> : ITagGroup
    {
        private readonly PLCType plcType= PLCType.Simens1200;
        /// <summary>
        /// 根据传入类型自动检测类型
        /// </summary>
        /// <param name="GroupName"></param>
        public TagGroup(string GroupName, PLCType pLCType)
        {
            plcType = pLCType;
            T t = default(T);
            this.GroupName = GroupName;
            Type type = t.GetType();
            
            if (type == typeof(bool))
            {
                this.dataType = TagDataType.Bool;
            }
            else if (type == typeof(short))
            {
                this.dataType = TagDataType.Short;
            }
            else if (type == typeof(int))
            {
                this.dataType = TagDataType.Int;
            }
            else if (type == typeof(float))
            {
                this.dataType = TagDataType.Float;
            }
            else if (type == typeof(string))
            {
                this.dataType = TagDataType.String;
            }
        }

        /// <summary>
        ///  组的名称,默认为ReadWrite
        /// </summary>
        public string GroupName { set; get; }
        /// <summary>
        ///  组描述信息
        /// </summary>
        public string GroupDiscription { set; get; }
        private TagDataType dataType = TagDataType.Int;
        /// <summary>
        /// 数据类型
        /// </summary>
        public TagDataType DataType
        {
            get
            {
                return dataType;

            }
            set => dataType = value;
        }
        private List<Tag<T>> _Tags = new List<Tag<T>>();
        /// <summary>
        ///  变量队列
        /// </summary>
        public List<Tag<T>> Tags
        {
            get { return _Tags; }
            set { _Tags = value; }
        }
        private List<BatchReadSection> _SectionList = new List<BatchReadSection>();
        /// <summary>
        /// 批量扫描的片段
        /// </summary>
        public List<BatchReadSection> SectionList
        {
            set { _SectionList = value; }
            get { return _SectionList; }
        }
        /// <summary>
        /// 加入连续型扫描片段
        /// </summary>
        /// <param name="tag"></param>
        public void AddToSectionList(Tag<T> tag)
        {
            Tag<T> previousTag;
            BatchReadSection batchReadSection;
            _Tags.Add(tag);
            if (!tag.CycleRead)//不需要扫描的，直接返回
            {
                return;
            }
            if (_Tags.Count == 1)//当前数量为1个，新增片段
            {
                batchReadSection = new BatchReadSection(tag.TagAddr, 0, 1);
                _SectionList.Add(batchReadSection);
            }
            else
            {
                previousTag = _Tags[_Tags.Count -2];//当前已经加入了
                if(TagContinuousCheck(previousTag, tag) && _SectionList[_SectionList.Count - 1].ReadLength<=128)//地址连续，且长度不大于128，在上一个片段内新增
                {
                    batchReadSection = _SectionList[_SectionList.Count - 1];
                    batchReadSection.ReadLength += 1;
                }
                else
                {
                    batchReadSection = new BatchReadSection(tag.TagAddr, _Tags.Count-1, 1);//片段断开，增加新的片段
                    _SectionList.Add(batchReadSection);
                }
            }
        }
        /// <summary>
        /// 标签地址连续性判断
        /// </summary>
        /// <returns></returns>
        public bool TagContinuousCheck(Tag<T> previousTag, Tag<T> currentTag)
        {
            switch(plcType)
            {
                case PLCType.Simens1200:
                case PLCType.Simens1500:
                case PLCType.Simens300:
                case PLCType.Simens200Smart:
                    return SimimensTagContinuousCheck(previousTag, currentTag);
                case PLCType.Omron:
                case PLCType.Fx:
                    return OmronFxTagContinuousCheck(previousTag, currentTag);
            }
            return true;
        }
        /// <summary>
        /// 西门子标签地址连续性判断
        /// </summary>
        /// <returns></returns>
        public bool SimimensTagContinuousCheck(Tag<T> previousTag, Tag<T> currentTag)
        {
            string addr1Str, addr2Str;
            string[] addr1Array, addr2Array;
            double addr1D, addr2D,addrSpan;
            addr1Str = previousTag.TagAddr.Replace("DB", "");
            addr2Str = currentTag.TagAddr.Replace("DB", "");
            addr1Str = previousTag.TagAddr.Replace("db", "");
            addr2Str = currentTag.TagAddr.Replace("db", "");

            addr1Array = addr1Str.Split('.');
            addr2Array = addr2Str.Split('.');
            
            if (currentTag.DataType==TagDataType.Bool)//DB1.0.0  DB1.0.1
            {
                if(addr1Array.Length==3 && addr2Array.Length==3)
                {
                    double.TryParse(addr1Array[2], out addr1D);
                    double.TryParse(addr2Array[2], out addr2D);
                    addrSpan = addr2D - addr1D;
                    if (addrSpan == 1)
                    {
                        return true;
                    }
                }
            }
            else if (currentTag.DataType == TagDataType.Short)//DB1.10 DB1.12
            {
                if (addr1Array.Length == 2 && addr2Array.Length == 2)
                {
                    double.TryParse(addr1Array[1], out addr1D);
                    double.TryParse(addr2Array[1], out addr2D);
                    addrSpan = addr2D - addr1D;
                    if (addrSpan == 2)
                    {
                        return true;
                    }
                }
            }
            else if (currentTag.DataType == TagDataType.Int || currentTag.DataType == TagDataType.Float)//DB1.1092 DB1.1096
            {
                if (addr1Array.Length == 2 && addr2Array.Length == 2)
                {
                    double.TryParse(addr1Array[1], out addr1D);
                    double.TryParse(addr2Array[1], out addr2D);
                    addrSpan = addr2D - addr1D;
                    if (addrSpan == 4)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        ///欧姆龙标签地址连续性判断
        /// </summary>
        /// <returns></returns>
        public bool OmronFxTagContinuousCheck(Tag<T> previousTag, Tag<T> currentTag)
        {
            string addr1Str, addr2Str;
            double addr1D, addr2D, addrSpan;
            addr1Str = previousTag.TagAddr.Replace("D", "");
            addr2Str = currentTag.TagAddr.Replace("D", "");
            addr1Str = previousTag.TagAddr.Replace("d", "");
            addr2Str = currentTag.TagAddr.Replace("d", "");

            if (currentTag.DataType == TagDataType.Bool)//DB1.0.0  DB1.0.1
            {
                
            }
            else if (currentTag.DataType == TagDataType.Short)//DB1.10 DB1.12
            {
                double.TryParse(addr1Str, out addr1D);
                double.TryParse(addr2Str, out addr2D);
                addrSpan = addr2D - addr1D;
                if (addrSpan == 2)
                {
                    return true;
                }
            }
            else if (currentTag.DataType == TagDataType.Int || currentTag.DataType == TagDataType.Float)//DB1.1092 DB1.1096
            {
                
            }
            return true;
        }
    }
    /// <summary>
    /// 标签组批量读写组
    /// </summary>
    public class BatchReadSection
    {
        public BatchReadSection(string Addr,int Index,int Length=1)
        {
            this.StartAddr = Addr;
            this.StartIndex = Index;
            this.ReadLength = (ushort)Length;
        }
        /// <summary>
        /// 起始的地址
        /// </summary>
        public string StartAddr { set; get; }
        /// <summary>
        /// 需要读取的长度，默认为1
        /// </summary>
        public ushort ReadLength { set; get; }
        /// <summary>
        /// 该分组所对应的位置
        /// </summary>
        public int StartIndex { set; get; }
    }
}
