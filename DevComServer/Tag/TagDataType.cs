using System;

namespace iFactory.DevComServer
{
    /// <summary>
    /// 标签数据类型
    /// </summary>
    public enum TagDataType : int
    {
        /// <summary>
        /// 位
        /// </summary>
        Bool = 0,
        /// <summary>
        /// short
        /// </summary>
        Short = 1,
        /// <summary>
        /// 无符号整型16
        /// </summary>
        Int = 2,
        /// <summary>
        /// 浮点32位
        /// </summary>
        Float = 3,
        /// <summary>
        /// 文本类型
        /// </summary>
        String = 4
    }
}
