using System.Collections.Generic;

namespace iFactory.DevComServer
{
    /// <summary>
    /// 标签组批量读写组
    /// </summary>
    public class BatchReadSection
    {
        public BatchReadSection(string Addr, int TagsIndex, int Length = 1)
        {
            this.StartAddr = Addr;
            this.ReadLength = (ushort)Length;
            this.Add(TagsIndex,0);//新增一个位置为0的
        }
        /// <summary>
        /// 读取的PLC起始的地址
        /// </summary>
        public string StartAddr { set; get; }
        /// <summary>
        /// 需要读取的长度，默认为1
        /// </summary>
        public ushort ReadLength { set; get; }
        /// <summary>
        /// 标签值在Section值的位置.Key是Tags队列位置，value是返回值的位置
        /// </summary>
        public List<TagIndexAndPos> TagValuePosList { set; get; } = new List<TagIndexAndPos>();
        /// <summary>
        /// 新增新的序号与位置对应
        /// </summary>
        public void Add(int index, int pos)
        {
            TagIndexAndPos tagIndexAndPos = new TagIndexAndPos(index, pos);
            this.TagValuePosList.Add(tagIndexAndPos);
        }
    }
    /// <summary>
    /// 标签序号与区域位置对应类
    /// </summary>
    public class TagIndexAndPos
    {
        public TagIndexAndPos(int index,int pos)
        {
            this.Index = index;
            this.Pos = pos;
        }
        /// <summary>
        /// 对应标签队列的序号
        /// </summary>
        public int Index { set; get; }
        /// <summary>
        /// 对应返回值区域的位置
        /// </summary>
        public int Pos { set; get; }
    }
}
