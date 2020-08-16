using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFactory.DevComServer
{
    public class TagList
    {
        /// <summary>
        /// PLC与标签对象集合
        /// </summary>
        public static List<PLCScanTask> PLCGroups { set; get; } = new List<PLCScanTask>();

        #region 获取各种类型的标签
        /// <summary>
        /// 获取标签
        /// </summary>
        /// <param name="Name">标签名称</param>
        /// <returns></returns>
        public static bool GetTag(string TagName, out Tag<bool> TagObject)
        {
            TagObject = null;
            foreach (var item in PLCGroups)
            {
                TagGroup<bool> tagGroup = item.PLCGroupObj.TagGroups.FirstOrDefault(x => x.DataType == TagDataType.Bool) as TagGroup<bool>;
                if (tagGroup != null)
                {
                    if (tagGroup.Tags.Any(x => x.TagName == TagName))
                    {
                        TagObject = tagGroup.Tags.FirstOrDefault(x => x.TagName == TagName);
                        return true;
                    }
                }
            }

            return false;
        }
        /// <summary>
        /// 获取标签
        /// </summary>
        /// <param name="Name">标签名称</param>
        /// <returns></returns>
        public static bool GetTag(string TagName, out Tag<short> TagObject)
        {
            TagObject = null;
            foreach (var item in PLCGroups)
            {
                TagGroup<short> tagGroup = item.PLCGroupObj.TagGroups.FirstOrDefault(x => x.DataType == TagDataType.Short) as TagGroup<short>;
                if (tagGroup != null)
                {
                    if (tagGroup.Tags.Any(x => x.TagName == TagName))
                    {
                        TagObject = tagGroup.Tags.FirstOrDefault(x => x.TagName == TagName);
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 获取标签
        /// </summary>
        /// <param name="Name">标签名称</param>
        /// <returns></returns>
        public static bool GetTag(string TagName, out Tag<int> TagObject)
        {
            TagObject = null;
            foreach (var item in PLCGroups)
            {
                TagGroup<int> tagGroup = item.PLCGroupObj.TagGroups.FirstOrDefault(x => x.DataType == TagDataType.Int) as TagGroup<int>;
                if (tagGroup != null)
                {
                    if (tagGroup.Tags.Any(x => x.TagName == TagName))
                    {
                        TagObject = tagGroup.Tags.FirstOrDefault(x => x.TagName == TagName);
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 获取标签
        /// </summary>
        /// <param name="Name">标签名称</param>
        /// <returns></returns>
        public static bool GetTag(string TagName, out Tag<float> TagObject)
        {
            TagObject = null;
            foreach (var item in PLCGroups)
            {
                TagGroup<float> tagGroup = item.PLCGroupObj.TagGroups.FirstOrDefault(x => x.DataType == TagDataType.Float) as TagGroup<float>;
                if (tagGroup != null)
                {
                    if (tagGroup.Tags.Any(x => x.TagName == TagName))
                    {
                        TagObject = tagGroup.Tags.FirstOrDefault(x => x.TagName == TagName);
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 获取标签
        /// </summary>
        /// <param name="Name">标签名称</param>
        /// <returns></returns>
        public static bool GetTag(string TagName, out Tag<string> TagObject)
        {
            TagObject = null;
            foreach (var item in PLCGroups)
            {
                TagGroup<string> tagGroup = item.PLCGroupObj.TagGroups.FirstOrDefault(x => x.DataType == TagDataType.String) as TagGroup<string>;
                if (tagGroup != null)
                {
                    if (tagGroup.Tags.Any(x => x.TagName == TagName))
                    {
                        TagObject = tagGroup.Tags.FirstOrDefault(x => x.TagName == TagName);
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion
    }
}
