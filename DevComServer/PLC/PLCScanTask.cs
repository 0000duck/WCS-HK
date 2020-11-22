using iFactory.CommonLibrary;
using iFactory.CommonLibrary.Interface;
using System;
using System.Threading;

namespace iFactory.DevComServer
{
    public class PLCScanTask:IDisposable
    {
        private readonly ILogWrite _log;
        private Thread _WorkTh;
        /// <summary>
        /// PLC访问驱动
        /// </summary>
        public IPLCHelper plcDriverHelper { set; get; }
        /// <summary>
        /// plc对象
        /// </summary>
        public PLCDevice PlcDevice { set; get; }

        public PLCScanTask(PLCDevice plcDevice,ILogWrite logWrite)
        {
            _log = logWrite;
            PlcDevice = plcDevice;
            try
            {
                switch(plcDevice.Type)
                {
                    case PLCType.Simens1200:
                    case PLCType.Simens1500:
                    case PLCType.Simens300:
                    case PLCType.Simens200Smart:
                        plcDriverHelper = new SimensPLCHelper(plcDevice,_log);//指定为西门子PLC连接
                        break;
                    case PLCType.Omron:
                        plcDriverHelper = new OmronPLCHelper(plcDevice, _log);//指定为欧姆龙PLC连接
                        break;
                    case PLCType.Fx:
                        plcDriverHelper = new FxPLCHelper(plcDevice, _log);//指定为三菱PLC连接
                        break;
                    case PLCType.Modbus:
                        plcDriverHelper = new ModbusTcpHelper(plcDevice, _log);//指定为Modbus连接(Robot机械手)
                        break;
                    case PLCType.TcpServer:
                        plcDriverHelper = new TcpServer(plcDevice, _log);//指定为Modbus连接(Robot机械手)
                        break;
                    default://默认西门子
                        plcDriverHelper = new SimensPLCHelper(plcDevice, _log);//指定为西门子PLC连接
                        
                        break;
                }
            }
            catch(Exception ex)
            {
                _log.WriteLog(ex.Message);
            }
        }
        /// <summary>
        /// 初始化变量以及扫描
        /// </summary>
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
            foreach(ITagGroup tagGroup in PlcDevice.TagGroups)
            {
                switch(tagGroup.DataType)
                {
                    case TagDataType.Bool:
                        TagGroup<bool> boolGroup = tagGroup as TagGroup<bool>;
                        foreach(var item in boolGroup.Tags)
                        {
                            item.WriteValueEvent += plcDriverHelper.WriteValue;
                        }
                        break;
                    case TagDataType.Short:
                        TagGroup<short> shortGroup = tagGroup as TagGroup<short>;
                        foreach (var item in shortGroup.Tags)
                        {
                            item.WriteValueEvent += plcDriverHelper.WriteValue;
                        }
                        break;
                    case TagDataType.Int:
                        TagGroup<int> intGroup = tagGroup as TagGroup<int>;
                        foreach (var item in intGroup.Tags)
                        {
                            item.WriteValueEvent += plcDriverHelper.WriteValue;
                        }
                        break;
                    case TagDataType.Float:
                        TagGroup<float> floatGroup = tagGroup as TagGroup<float>;
                        foreach (var item in floatGroup.Tags)
                        {
                            item.WriteValueEvent += plcDriverHelper.WriteValue;
                        }
                        break;
                    case TagDataType.String://暂时不支持
                        TagGroup<string> stringGroup = tagGroup as TagGroup<string>;
                        foreach (var item in stringGroup.Tags)
                        {
                            item.WriteValueEvent += plcDriverHelper.WriteValue;
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
            plcDriverHelper.ConnectToPlc();//连接放在线程里面，开始连接

            while (true)
            {
                Console.WriteLine($"周期任务：{PlcDevice.Name}-{PlcDevice.Ip}");
                plcDriverHelper.CheckConnect();
                if (PlcDevice.IsConnected)
                {
                    foreach (ITagGroup tagGroup in PlcDevice.TagGroups)
                    {
                        switch (tagGroup.DataType)
                        {
                            case TagDataType.Bool:
                                TagGroup<bool> boolGroup = tagGroup as TagGroup<bool>;
                                foreach(BatchReadSection section in boolGroup.SectionList)
                                {
                                    bool[] values;
                                    if(plcDriverHelper.BatchReadValue(section.StartAddr, section.ReadLength,out values))
                                    {
                                        for(int i=0;i< section.TagValuePosList.Count; i++)
                                        {
                                            TagIndexAndPos indexPost = section.TagValuePosList[i];
                                            boolGroup.Tags[indexPost.Index].TagValue = values[indexPost.Pos];
                                        }
                                    }
                                }
                                break;
                            case TagDataType.Short:
                                TagGroup<short> shortGroup = tagGroup as TagGroup<short>;
                                foreach (BatchReadSection section in shortGroup.SectionList)
                                {
                                    short[] values;
                                    if (plcDriverHelper.BatchReadValue(section.StartAddr, section.ReadLength, out values))
                                    {
                                        for (int i = 0; i < section.TagValuePosList.Count; i++)
                                        {
                                            TagIndexAndPos indexPost = section.TagValuePosList[i];
                                            shortGroup.Tags[indexPost.Index].TagValue = values[indexPost.Pos];
                                        }
                                    }
                                }
                                break;
                            case TagDataType.Int:
                                TagGroup<int> intGroup = tagGroup as TagGroup<int>;
                                foreach (BatchReadSection section in intGroup.SectionList)
                                {
                                    int[] values;
                                    if (plcDriverHelper.BatchReadValue(section.StartAddr, section.ReadLength, out values))
                                    {
                                        for (int i = 0; i < section.TagValuePosList.Count; i++)
                                        {
                                            TagIndexAndPos indexPost = section.TagValuePosList[i];
                                            intGroup.Tags[indexPost.Index].TagValue = values[indexPost.Pos];
                                        }
                                    }
                                }
                                break;
                            case TagDataType.Float:
                                TagGroup<float> floatGroup = tagGroup as TagGroup<float>;
                                foreach (BatchReadSection section in floatGroup.SectionList)
                                {
                                    float[] values;
                                    if (plcDriverHelper.BatchReadValue(section.StartAddr, section.ReadLength, out values))
                                    {
                                        for (int i = 0; i < section.TagValuePosList.Count; i++)
                                        {
                                            TagIndexAndPos indexPost = section.TagValuePosList[i];
                                            floatGroup.Tags[indexPost.Index].TagValue = values[indexPost.Pos];
                                        }
                                    }
                                }
                                break;
                            case TagDataType.String:
                                TagGroup<string> stringGroup = tagGroup as TagGroup<string>;
                                foreach (BatchReadSection section in stringGroup.SectionList)
                                {
                                    string[] values;
                                    if (plcDriverHelper.BatchReadValue(section.StartAddr, section.ReadLength, out values))
                                    {
                                        for (int i = 0; i < section.TagValuePosList.Count; i++)
                                        {
                                            TagIndexAndPos indexPost = section.TagValuePosList[i];
                                            stringGroup.Tags[indexPost.Index].TagValue = values[indexPost.Pos];
                                        }
                                    }
                                }
                                break;
                        }
                    }
                    if(!string.IsNullOrEmpty(PlcDevice.HeartBit))//写入心跳位
                    {
                        plcDriverHelper.WriteValue(PlcDevice.HeartBit, true);
                    }
                }
                else
                {
                    if (NetworkHelper.IsNetWorkConnect(PlcDevice.Ip))//plc可以ping通
                    {
                        Thread.Sleep(1000);
                        _log.WriteLog($"{PlcDevice.Name} {PlcDevice.Ip}重新连接并初始化！");
                        plcDriverHelper.ConnectToPlc();
                    }
                    else
                    {
                        _log.WriteLog($"{PlcDevice.Name} {PlcDevice.Ip}连接断开，等待再次连接！");
                        Thread.Sleep(5000);//延时等待PLC再次连接
                    }
                }
                Thread.Sleep(PlcDevice.CycleTime);
            }
        }

        #region 直接读取标签
        /// <summary>
        /// 读取float变量
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool ReadTag(Tag<float> tag)
        {
            float value;
            if (plcDriverHelper.ReadValue(tag.TagAddr, out value))
            {
                tag.TagValue = value;
                return true;
            }
            return false;
        }
        /// <summary>
        /// 读取bool变量
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool ReadTag(Tag<bool> tag)
        {
            bool value;
            if (plcDriverHelper.ReadValue(tag.TagAddr, out value))
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
            if (plcDriverHelper.ReadValue(tag.TagAddr, out value))
            {
                tag.TagValue = value;
                return true;
            }
            return false;
        }
        /// <summary>
        /// 读取int变量
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool ReadTag(Tag<int> tag)
        {
            int value;
            if (plcDriverHelper.ReadValue(tag.TagAddr, out value))
            {
                tag.TagValue = value;
                return true;
            }
            return false;
        }
        #endregion

        public void Dispose()
        {
            try
            {
                _WorkTh.Abort();
            }
            catch (Exception ex)
            {

            }
            plcDriverHelper.Dispose();
        }
    }
}
