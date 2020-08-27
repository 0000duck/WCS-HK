using System;
using System.Collections.Generic;
using System.Linq;

namespace iFactory.DevComServer
{
    public class TagList
    {
        /// <summary>
        /// PLC与标签对象集合
        /// </summary>
        public static List<PLCScanTask> PLCGroups { set; get; } = new List<PLCScanTask>();
        /// <summary>
        /// 获取标签
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tagGroup"></param>
        /// <param name="TagName"></param>
        /// <param name="TagObject"></param>
        /// <returns></returns>
        private static bool GetTag<T>(TagGroup<T> tagGroup, string TagName, out Tag<T> TagObject)
        {
            TagObject = null;
            if (tagGroup != null)
            {
                if (tagGroup.Tags.Any(x => x.TagName == TagName))
                {
                    TagObject = tagGroup.Tags.FirstOrDefault(x => x.TagName == TagName);
                    return true;
                }
            }
            return false;
        }
        #region 获取各种类型的标签
        /// <summary>
        /// 获取标签
        /// </summary>
        /// <param name="Name">标签名称</param>
        /// <returns></returns>
        public static bool GetTag(string TagName, out Tag<bool> TagObject, string PlcName = "")
        {
            TagObject = null;
            if(string.IsNullOrEmpty(PlcName))
            {
                foreach (var item in PLCGroups)
                {
                    TagGroup<bool> tagGroup = item.PlcDevice.TagGroups.FirstOrDefault(x => x.DataType == TagDataType.Bool) as TagGroup<bool>;
                    if( GetTag<bool>(tagGroup, TagName, out TagObject))
                    {
                        return true;
                    }
                }
            }
            else
            {
                var plc = PLCGroups.FirstOrDefault(x=>x.PlcDevice.Name== PlcName);
                if (plc != null)
                {
                    TagGroup<bool> tagGroup = plc.PlcDevice.TagGroups.FirstOrDefault(x => x.DataType == TagDataType.Bool) as TagGroup<bool>;
                    return GetTag<bool>(tagGroup, TagName,out TagObject);

                }
            }

            return false;
        }
        /// <summary>
        /// 获取标签
        /// </summary>
        /// <param name="Name">标签名称</param>
        /// <returns></returns>
        public static bool GetTag(string TagName, out Tag<short> TagObject, string PlcName = "")
        {
            TagObject = null;
            if (string.IsNullOrEmpty(PlcName))
            {
                foreach (var item in PLCGroups)
                {
                    TagGroup<short> tagGroup = item.PlcDevice.TagGroups.FirstOrDefault(x => x.DataType == TagDataType.Short) as TagGroup<short>;
                    if (GetTag<short>(tagGroup, TagName, out TagObject))
                    {
                        return true;
                    }
                }
            }
            else
            {
                var plc = PLCGroups.FirstOrDefault(x => x.PlcDevice.Name == PlcName);
                if (plc != null)
                {
                    TagGroup<short> tagGroup = plc.PlcDevice.TagGroups.FirstOrDefault(x => x.DataType == TagDataType.Short) as TagGroup<short>;
                    return GetTag<short>(tagGroup, TagName, out TagObject);

                }
            }
            return false;
        }
        /// <summary>
        /// 获取标签
        /// </summary>
        /// <param name="Name">标签名称</param>
        /// <returns></returns>
        public static bool GetTag(string TagName, out Tag<int> TagObject, string PlcName = "")
        {
            TagObject = null;
            if (string.IsNullOrEmpty(PlcName))
            {
                foreach (var item in PLCGroups)
                {
                    TagGroup<int> tagGroup = item.PlcDevice.TagGroups.FirstOrDefault(x => x.DataType == TagDataType.Int) as TagGroup<int>;
                    if (GetTag<int>(tagGroup, TagName, out TagObject))
                    {
                        return true;
                    }
                }
            }
            else
            {
                var plc = PLCGroups.FirstOrDefault(x => x.PlcDevice.Name == PlcName);
                if (plc != null)
                {
                    TagGroup<int> tagGroup = plc.PlcDevice.TagGroups.FirstOrDefault(x => x.DataType == TagDataType.Int) as TagGroup<int>;
                    return GetTag<int>(tagGroup, TagName, out TagObject);

                }
            }
            return false;
        }
        /// <summary>
        /// 获取标签
        /// </summary>
        /// <param name="Name">标签名称</param>
        /// <returns></returns>
        public static bool GetTag(string TagName, out Tag<float> TagObject, string PlcName = "")
        {
            TagObject = null;
            if (string.IsNullOrEmpty(PlcName))
            {
                foreach (var item in PLCGroups)
                {
                    TagGroup<float> tagGroup = item.PlcDevice.TagGroups.FirstOrDefault(x => x.DataType == TagDataType.Float) as TagGroup<float>;
                    if (GetTag<float>(tagGroup, TagName, out TagObject))
                    {
                        return true;
                    }
                }
            }
            else
            {
                var plc = PLCGroups.FirstOrDefault(x => x.PlcDevice.Name == PlcName);
                if (plc != null)
                {
                    TagGroup<float> tagGroup = plc.PlcDevice.TagGroups.FirstOrDefault(x => x.DataType == TagDataType.Float) as TagGroup<float>;
                    return GetTag<float>(tagGroup, TagName, out TagObject);

                }
            }
            return false;
        }
        /// <summary>
        /// 获取标签
        /// </summary>
        /// <param name="Name">标签名称</param>
        /// <returns></returns>
        public static bool GetTag(string TagName, out Tag<string> TagObject, string PlcName = "")
        {
            TagObject = null;
            if (string.IsNullOrEmpty(PlcName))
            {
                foreach (var item in PLCGroups)
                {
                    TagGroup<string> tagGroup = item.PlcDevice.TagGroups.FirstOrDefault(x => x.DataType == TagDataType.String) as TagGroup<string>;
                    if (GetTag<string>(tagGroup, TagName, out TagObject))
                    {
                        return true;
                    }
                }
            }
            else
            {
                var plc = PLCGroups.FirstOrDefault(x => x.PlcDevice.Name == PlcName);
                if (plc != null)
                {
                    TagGroup<string> tagGroup = plc.PlcDevice.TagGroups.FirstOrDefault(x => x.DataType == TagDataType.String) as TagGroup<string>;
                    return GetTag<string>(tagGroup, TagName, out TagObject);

                }
            }
            return false;
        }
        #endregion
    }
}
