using iFactory.CommonLibrary;
using iFactory.CommonLibrary.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;

namespace iFactory.DevComServer
{
    public class PLCScanTask:IDisposable
    {
        public IPLCHelper plcHelper { get; set; }
        private Thread _WorkTh;
        private readonly ILogWrite _log;
        /// <summary>
        /// plc组对象
        /// </summary>
        public PLCGroup PLCGroupObj { set; get; }

        public PLCScanTask(PLCGroup plcGroup,ILogWrite logWrite)
        {
            _log = logWrite;
            PLCGroupObj = plcGroup;
            try
            {
                switch(plcGroup.Type)
                {
                    case PLCType.Simens1200:
                    case PLCType.Simens1500:
                    case PLCType.Simens300:
                    case PLCType.Simens200Smart:
                        plcHelper = new SimensPLCHelper(_log);//指定为西门子PLC连接
                        plcHelper.ConnectToPlc(PLCGroupObj.Ip, PLCGroupObj.Port, plcGroup.Type);
                        break;
                    case PLCType.Omron:
                        plcHelper = new OmronPLCHelper(_log);//指定为欧姆龙PLC连接
                        plcHelper.ConnectToPlc(PLCGroupObj.Ip, PLCGroupObj.Port);
                        break;
                    case PLCType.Fx:
                        plcHelper = new FxPLCHelper(_log);//指定为三菱PLC连接
                        plcHelper.ConnectToPlc(PLCGroupObj.Ip, PLCGroupObj.Port);
                        break;
                    default://默认西门子
                        plcHelper = new SimensPLCHelper(_log);//指定为西门子PLC连接
                        plcHelper.ConnectToPlc(PLCGroupObj.Ip, PLCGroupObj.Port, plcGroup.Type);
                        break;
                }
              
               // StartTask();
            }
            catch(Exception ex)
            {
                _log.WriteLog(ex.Message);
            }
        }
        public void StartTask()
        {
            TagListInitial();
            _WorkTh = new Thread(CycReadTagTask);
            _WorkTh.IsBackground = true;
            _WorkTh.Start();
        }
        /// <summary>
        /// 标签队列初始化
        /// </summary>
        public void TagListInitial()
        {
            foreach(ITagGroup tagGroup in PLCGroupObj.TagGroups)
            {
                switch(tagGroup.DataType)
                {
                    case TagDataType.Bool:
                        TagGroup<bool> boolGroup = tagGroup as TagGroup<bool>;
                        foreach(var item in boolGroup.Tags)
                        {
                            item.WriteValueEvent += WriteValue;
                        }
                        break;
                    case TagDataType.Short:
                        TagGroup<short> shortGroup = tagGroup as TagGroup<short>;
                        foreach (var item in shortGroup.Tags)
                        {
                            item.WriteValueEvent += WriteValue;
                        }
                        break;
                    case TagDataType.Int:
                        TagGroup<int> intGroup = tagGroup as TagGroup<int>;
                        foreach (var item in intGroup.Tags)
                        {
                            item.WriteValueEvent += WriteValue;
                        }
                        break;
                    case TagDataType.Float:
                        TagGroup<float> floatGroup = tagGroup as TagGroup<float>;
                        foreach (var item in floatGroup.Tags)
                        {
                            item.WriteValueEvent += WriteValue;
                        }
                        break;
                    case TagDataType.String:
                        TagGroup<string> stringGroup = tagGroup as TagGroup<string>;
                        foreach (var item in stringGroup.Tags)
                        {
                            item.WriteValueEvent += WriteValue;
                        }
                        break;
                }
            }
        }
        
        /// <summary>
        /// 周期任务
        /// </summary>
        private void CycReadTagTask()
        {
            Thread.Sleep(3000);//延时等待其他任务加载，然后再启动刷新
            while (true)
            {
                PLCGroupObj.IsConnected = plcHelper.CheckConnect();
                if (PLCGroupObj.IsConnected)
                {
                    foreach (ITagGroup tagGroup in PLCGroupObj.TagGroups)
                    {
                        switch (tagGroup.DataType)
                        {
                            case TagDataType.Bool:
                                TagGroup<bool> boolGroup = tagGroup as TagGroup<bool>;
                                foreach(BatchReadSection section in boolGroup.SectionList)
                                {
                                    bool[] values;
                                    if(plcHelper.BatchReadValue(section.StartAddr, section.ReadLength,out values))
                                    {
                                        for(int i=0;i< values.Length;i++)
                                        {
                                            boolGroup.Tags[section.StartIndex + i].TagValue = values[i];
                                        }
                                    }
                                }
                                break;
                            case TagDataType.Short:
                                TagGroup<short> shortGroup = tagGroup as TagGroup<short>;
                                foreach (BatchReadSection section in shortGroup.SectionList)
                                {
                                    short[] values;
                                    if (plcHelper.BatchReadValue(section.StartAddr, section.ReadLength, out values))
                                    {
                                        for (int i = 0; i < values.Length; i++)
                                        {
                                            shortGroup.Tags[section.StartIndex + i].TagValue = values[i];
                                        }
                                    }
                                }
                                break;
                            case TagDataType.Int:
                                TagGroup<int> intGroup = tagGroup as TagGroup<int>;
                                foreach (BatchReadSection section in intGroup.SectionList)
                                {
                                    int[] values;
                                    if (plcHelper.BatchReadValue(section.StartAddr, section.ReadLength, out values))
                                    {
                                        for (int i = 0; i < values.Length; i++)
                                        {
                                            intGroup.Tags[section.StartIndex + i].TagValue = values[i];
                                        }
                                    }
                                }
                                break;
                            case TagDataType.Float:
                                TagGroup<float> floatGroup = tagGroup as TagGroup<float>;
                                foreach (BatchReadSection section in floatGroup.SectionList)
                                {
                                    float[] values;
                                    if (plcHelper.BatchReadValue(section.StartAddr, section.ReadLength, out values))
                                    {
                                        for (int i = 0; i < values.Length; i++)
                                        {
                                            floatGroup.Tags[section.StartIndex + i].TagValue = values[i];
                                        }
                                    }
                                }
                                break;
                            case TagDataType.String:
                                TagGroup<string> stringGroup = tagGroup as TagGroup<string>;
                                foreach (BatchReadSection section in stringGroup.SectionList)
                                {
                                    string[] values;
                                    if (plcHelper.BatchReadValue(section.StartAddr, section.ReadLength, out values))
                                    {
                                        for (int i = 0; i < values.Length; i++)
                                        {
                                            stringGroup.Tags[section.StartIndex + i].TagValue = values[i];
                                        }
                                    }
                                }
                                break;
                        }
                    }
                    if(!string.IsNullOrEmpty(PLCGroupObj.HeartBit))//写入心跳位
                    {
                        plcHelper.WriteValue(PLCGroupObj.HeartBit, true);
                    }
                }
                else
                {
                    if (NetworkHelper.IsNetWorkConnect(PLCGroupObj.Ip))//plc可以ping通
                    {
                        Thread.Sleep(1000);
                        _log.WriteLog("PLC重新连接初始化！");
                        plcHelper.ReConnectToPlc();
                    }
                    else
                    {
                        _log.WriteLog("PLC连接断开，等待再次连接！");
                        Thread.Sleep(5000);//延时等待PLC再次连接
                    }
                }
                Thread.Sleep(PLCGroupObj.CycleTime);
            }
        }
        #region 标签写入值
        public void WriteValue(Tag<bool> tag, bool value,int Length=-1)
        {
            bool res = plcHelper.WriteValue(tag.TagAddr, value);
            if (!res)
            {
                _log.WriteLog($"{tag.TagName}={tag.TagAddr}写入值{value}失败，重新写入");
                plcHelper.WriteValue(tag.TagAddr, value);
            }
        }
        public void WriteValue(Tag<short> tag, short value, int Length = -1)
        {
            bool res = plcHelper.WriteValue(tag.TagAddr, value);
            if (!res)
            {
                _log.WriteLog($"{tag.TagName}={tag.TagAddr}写入值{value}失败，重新写入");
                plcHelper.WriteValue(tag.TagAddr, value);
            }
        }
        public void WriteValue(Tag<int> tag, int value, int Length = -1)
        {
            bool res = plcHelper.WriteValue(tag.TagAddr, value);
            if (!res)
            {
                _log.WriteLog($"{tag.TagName}={tag.TagAddr}写入值{value}失败，重新写入");
                plcHelper.WriteValue(tag.TagAddr, value);
            }
        }
        public void WriteValue(Tag<float> tag, float value, int Length = -1)
        {
            bool res = plcHelper.WriteValue(tag.TagAddr, value);
            if (!res)
            {
                _log.WriteLog($"{tag.TagName}={tag.TagAddr}写入值{value}失败，重新写入");
                plcHelper.WriteValue(tag.TagAddr, value);
            }
        }
        public void WriteValue(Tag<string> tag, string value, int Length = -1)
        {
            //_plcHelper.WriteValue(tag.TagAddr, value, WCharMode, Length);
        }
        
        #endregion

        #region 读取变量
        ///// <summary>
        ///// 读取文本变量
        ///// </summary>
        ///// <param name="tag"></param>
        ///// <returns></returns>
        //public static bool ReadTag(Tag<string> tag)
        //{
        //    string value;
        //    if(PLCHelper.ReadValue(tag.TagAddr, out value, (ushort)tag.Length, true))
        //    {
        //        tag.TagValue = value;
        //        return true;
        //    }
        //    return false;
        //}
        ///// <summary>
        ///// 读取float变量
        ///// </summary>
        ///// <param name="tag"></param>
        ///// <returns></returns>
        //public static bool ReadTag(Tag<float> tag)
        //{
        //    float value;
        //    if (PLCHelper.ReadValue(tag.TagAddr, out value))
        //    {
        //        tag.TagValue = value;
        //        return true;
        //    }
        //    return false;
        //}
        /// <summary>
        /// 读取bool变量
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool ReadTag(Tag<bool> tag)
        {
            bool value;
            if (plcHelper.ReadValue(tag.TagAddr, out value))
            {
                tag.TagValue = value;
                return true;
            }
            return false;
        }
        /// <summary>
        /// 读取short变量
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool ReadTag(Tag<short> tag)
        {
            short value;
            if (plcHelper.ReadValue(tag.TagAddr, out value))
            {
                tag.TagValue = value;
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            plcHelper.Dispose();
        }
        ///// <summary>
        ///// 读取bool变量
        ///// </summary>
        ///// <param name="tag"></param>
        ///// <returns></returns>
        //public static bool ReadTag(Tag<short> tag)
        //{
        //    short value;
        //    if (PLCHelper.ReadValue(tag.TagAddr, out value))
        //    {
        //        tag.TagValue = value;
        //        return true;
        //    }
        //    return false;
        //}
        #endregion
    }
}
