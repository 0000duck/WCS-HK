using iFactory.CommonLibrary.Interface;
using iFactory.DataService.IService;
using iFactory.DataService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFactory.DevComServer
{
    public class PLCManager
    {
        private static IDatabaseTagGroupService _plcGroupService;
        private static IDatabaseTagService _tagService;
        private static ILogWrite logWrite;
       
        /// <summary>
        /// 设置服务和启动
        /// </summary>
        /// <param name="pLCGroupService"></param>
        /// <param name="tagService"></param>
        /// <param name="logWriteObj"></param>
        public static void SetService(IDatabaseTagGroupService plcGroupService,
                                      IDatabaseTagService tagService,
                                      ILogWrite logWriteObj)
        {
            _plcGroupService = plcGroupService;
            _tagService = tagService;
            logWrite = logWriteObj;
            
            LoadTags();
        }
        
        /// <summary>
        /// 加载标签组和标签
        /// </summary>
        public static void LoadTags()
        {
            var list = _plcGroupService.QueryableToList(x=>x.Active==true);//查找出激活的PLC
            foreach(var plcgroupItem in list)
            {
                PLCGroup plcGroup = new PLCGroup();
                plcGroup.Name = plcgroupItem.Name;
                plcGroup.Ip = plcgroupItem.Ip;
                plcGroup.Type =(PLCType) plcgroupItem.DeviceType;
                plcGroup.Port = plcgroupItem.Port;
                plcGroup.CycleTime = plcgroupItem.CycleTime;
                plcGroup.HeartBit = plcgroupItem.HeartBit;
                plcGroup.Description = plcgroupItem.Description;

                var dbTags= _tagService.QueryableToList(x=>x.GroupId== plcgroupItem.id);
                
                foreach(var dbTag in dbTags)
                {
                    switch(dbTag.DataType)
                    {
                        case (int)TagDataType.Bool:
                            Tag<bool> boolTag = new Tag<bool>();
                            GetDbTagInfoAndAdd<bool>(plcGroup,dbTag, boolTag);
                            break;
                        case (int)TagDataType.Short:
                            Tag<short> shortTag = new Tag<short>();
                            GetDbTagInfoAndAdd<short>(plcGroup, dbTag, shortTag);
                            break;
                        case (int)TagDataType.Int:
                            Tag<int> intTag = new Tag<int>();
                            GetDbTagInfoAndAdd<int>(plcGroup, dbTag, intTag);
                            break;
                        case (int)TagDataType.Float:
                            Tag<float> floatTag = new Tag<float>();
                            GetDbTagInfoAndAdd<float>(plcGroup, dbTag, floatTag);
                            break;
                        case (int)TagDataType.String:
                            Tag<string> stringTag = new Tag<string>();
                            GetDbTagInfoAndAdd<string>(plcGroup, dbTag, stringTag);
                            break;
                    }
                }
                PLCScanTask scanTask = new PLCScanTask(plcGroup, logWrite);
                scanTask.StartTask();
                TagList.PLCGroups.Add(scanTask);
            }
        }
        /// <summary>
        /// 获取标签的属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="databaseTag"></param>
        /// <param name="tag"></param>
        private static void GetDbTagInfoAndAdd<T>(PLCGroup plcGroup,DatabaseTag databaseTag,Tag<T> tag)
        {
            // tag.DataType = (TagDataType)databaseTag.DataType;//类型自动判断了
            tag.CycleRead = databaseTag.CycleRead;
            tag.Length = databaseTag.Length;
            tag.TagName = databaseTag.TagName;
            tag.TagAddr = databaseTag.TagAddr;
            tag.GroupId = databaseTag.GroupId;

            plcGroup.AddToGroup<T>(tag);
        }
    }
}
