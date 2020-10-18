using iFactory.CommonLibrary.Interface;
using iFactory.DataService.IService;
using iFactory.DataService.Model;

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
            if (!HslCommunication.Authorization.SetAuthorizationCode("2487cfb8-334f-4b04-bd32-122453cfeb37"))
            {
                logWriteObj.WriteLog("访问驱动激活失败");
            }
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
                PLCDevice plcDevice = new PLCDevice();
                plcDevice.Name = plcgroupItem.Name;
                plcDevice.Ip = plcgroupItem.Ip;
                plcDevice.Type =(PLCType) plcgroupItem.DeviceType;
                plcDevice.Port = plcgroupItem.Port;
                plcDevice.CycleTime = plcgroupItem.CycleTime;
                plcDevice.HeartBit = plcgroupItem.HeartBit;
                plcDevice.Description = plcgroupItem.Description;

                var dbTags= _tagService.QueryableToList(x=>x.GroupId== plcgroupItem.id);
                
                foreach(var dbTag in dbTags)
                {
                    switch(dbTag.DataType)
                    {
                        case (int)TagDataType.Bool:
                            Tag<bool> boolTag = new Tag<bool>();
                            GetDbTagInfoAndAdd<bool>(plcDevice,dbTag, boolTag);
                            break;
                        case (int)TagDataType.Short:
                            Tag<short> shortTag = new Tag<short>();
                            GetDbTagInfoAndAdd<short>(plcDevice, dbTag, shortTag);
                            break;
                        case (int)TagDataType.Int:
                            Tag<int> intTag = new Tag<int>();
                            GetDbTagInfoAndAdd<int>(plcDevice, dbTag, intTag);
                            break;
                        case (int)TagDataType.Float:
                            Tag<float> floatTag = new Tag<float>();
                            GetDbTagInfoAndAdd<float>(plcDevice, dbTag, floatTag);
                            break;
                        case (int)TagDataType.String:
                            Tag<string> stringTag = new Tag<string>();
                            GetDbTagInfoAndAdd<string>(plcDevice, dbTag, stringTag);
                            break;
                    }
                }
                PLCScanTask scanTask = new PLCScanTask(plcDevice, logWrite);
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
        private static void GetDbTagInfoAndAdd<T>(PLCDevice plcDevice, DatabaseTag databaseTag,Tag<T> tag)
        {
            // tag.DataType = (TagDataType)databaseTag.DataType;//类型自动判断了
            tag.CycleRead = databaseTag.CycleRead;
            tag.Length = databaseTag.Length;
            tag.TagName = databaseTag.TagName;
            tag.TagAddr = databaseTag.TagAddr;
            tag.GroupId = databaseTag.GroupId;

            plcDevice.AddToGroup<T>(tag);
        }
    }
}
