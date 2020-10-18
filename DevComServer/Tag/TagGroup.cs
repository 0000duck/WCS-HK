using System;
using System.Collections.Generic;

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
            int pos = 0;
            Tag<T> previousTag;
            BatchReadSection batchReadSection;
            _Tags.Add(tag);
            if (!tag.CycleRead)//不需要扫描的，直接返回
            {
                return;
            }
            if (_Tags.Count == 1|| _SectionList.Count==0)//当前数量为1个，新增片段
            {
                pos = _Tags.Count - 1;
                batchReadSection = new BatchReadSection(tag.TagAddr, pos, 1);
                _SectionList.Add(batchReadSection);
            }
            else
            {
                batchReadSection = _SectionList[_SectionList.Count - 1];//上一个片段
                TagIndexAndPos tagIndexPos = batchReadSection.TagValuePosList[batchReadSection.TagValuePosList.Count - 1];//取出片段内的位置
                previousTag = _Tags[tagIndexPos.Index];//当前已经加入了
                var res = AddressOffsetCheck.TagContinuousCheck<T>(previousTag, tag, plcType);
                if (res.IsContinuous==true && _SectionList[_SectionList.Count - 1].ReadLength<=128)//地址连续，且长度不大于128，在上一个片段内新增
                {
                    batchReadSection.ReadLength += (ushort)res.OffsetLength;
                    pos = tagIndexPos.Pos + res.OffsetLength;//加上现在的偏移地址
                    batchReadSection.Add(_Tags.Count - 1, pos);//该标签在分片里面的位置
                }
                else
                {
                    batchReadSection = new BatchReadSection(tag.TagAddr, _Tags.Count-1, 1);//片段断开，增加新的片段
                    _SectionList.Add(batchReadSection);
                }
            }
        }
       
    }
   
}
