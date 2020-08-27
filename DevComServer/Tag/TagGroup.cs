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
                var res = TagContinuousCheck(previousTag, tag);
                if (res.Item1==true && _SectionList[_SectionList.Count - 1].ReadLength<=128)//地址连续，且长度不大于128，在上一个片段内新增
                {
                    batchReadSection = _SectionList[_SectionList.Count - 1];
                    batchReadSection.ReadLength += (ushort)res.Item2;
                    int pos = batchReadSection.TagPosList[batchReadSection.TagPosList.Count - 1];//前一个偏移地址
                    pos = pos + res.Item2;//加上现在的偏移地址
                    batchReadSection.TagPosList.Add(pos);//该标签在分片里面的位置
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
        /// <returns>地址连续返回True,否则返回False</returns>
        public (bool, int) TagContinuousCheck(Tag<T> previousTag, Tag<T> currentTag)
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
                case PLCType.Modbus:
                    return ModbusContinuousCheck(previousTag, currentTag);
            }
            return (true,0);
        }
        /// <summary>
        /// 西门子标签地址连续性判断
        /// </summary>
        /// <returns></returns>
        public (bool, int) SimimensTagContinuousCheck(Tag<T> previousTag, Tag<T> currentTag)
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
                        return (true, 1);
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
                        return (true,1);
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
                        return (true, 1);
                    }
                }
            }
            return (false, 0);
        }
        /// <summary>
        /// 欧姆龙标签地址连续性判断
        /// </summary>
        /// <param name="previousTag"></param>
        /// <param name="currentTag"></param>
        /// <returns>地址连续返回True,否则返回False.第二个参数返回间隔</returns>
        public (bool,int) OmronFxTagContinuousCheck(Tag<T> previousTag, Tag<T> currentTag)
        {
            string addr1Str, addr2Str;
            int addr1D, addr2D, addrSpan;
            addr1Str = previousTag.TagAddr.Replace("D", "");
            addr2Str = currentTag.TagAddr.Replace("D", "");
            addr1Str = previousTag.TagAddr.Replace("d", "");
            addr2Str = currentTag.TagAddr.Replace("d", "");

            if (currentTag.DataType == TagDataType.Bool)//
            {
                return (false,0);
            }
            else if (currentTag.DataType == TagDataType.Short)//D100 D102
            {
                int.TryParse(addr1Str, out addr1D);
                int.TryParse(addr2Str, out addr2D);
                addrSpan = addr2D - addr1D;
                if (addrSpan <= 2)//间隔2个以内
                {
                    return (true, addrSpan);
                }
            }
            else if (currentTag.DataType == TagDataType.Int || currentTag.DataType == TagDataType.Float)//DB1.1092 DB1.1096
            {
                
            }
            return (false,0);
        }
        /// <summary>
        ///Modbus地址连续性判断
        /// </summary>
        /// <returns>地址连续返回True,否则返回False</returns>
        public (bool, int) ModbusContinuousCheck(Tag<T> previousTag, Tag<T> currentTag)
        {
            return (false,0);
        }
    }
}
