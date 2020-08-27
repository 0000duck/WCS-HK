using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFactory.DevComServer
{
    /// <summary>
    /// 标签组批量读写组
    /// </summary>
    public class BatchReadSection
    {
        public BatchReadSection(string Addr, int Index, int Length = 1)
        {
            this.StartAddr = Addr;
            this.StartIndex = Index;
            this.ReadLength = (ushort)Length;
            this.TagPosList.Add(0);//新增一个位置为0的
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
        /// <summary>
        /// 标签在Section里面的位置
        /// </summary>
        public List<int> TagPosList { set; get; } = new List<int>();
    }
}
