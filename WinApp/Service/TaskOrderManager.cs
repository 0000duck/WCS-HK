using iFactory.CommonLibrary;
using iFactory.DataService.Model;
using iFactory.DevComServer;
using iFactoryApp.Common;
using iFactoryApp.ViewModel;
using Keyence.AutoID.SDK;
using RFIDLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;

namespace iFactoryApp.Service
{
    public class TaskOrderManager: ErrorMessageEventClass,IDisposable
    {
        private readonly ISystemLogViewModel _systemLogViewModel;
        private readonly TaskOrderViewModel _taskOrderViewModel;
        private readonly RFIDViewModel _RFIDViewModel;
        public List<KeyenceCameraHelper> cameraList = new List<KeyenceCameraHelper>();
        private Tag<short> RFID_SigTag, RFID_WriteTag;
        private Tag<short> SnSig1Tag, SnSig2Tag, SnDeal1Tag, SnDeal2Tag;
        private readonly DispatcherTimer timer;

        public TaskOrderManager(ISystemLogViewModel systemLogViewModel,
                                TaskOrderViewModel taskOrderViewModel,
                                RFIDViewModel RfidViewModel)
        {
            _systemLogViewModel = systemLogViewModel;
            _taskOrderViewModel = taskOrderViewModel;
            _RFIDViewModel = RfidViewModel;
         
            TagInitial();
            timer = new DispatcherTimer() { IsEnabled=true,Interval = TimeSpan.FromSeconds(5) };//5秒周期读取产量数据
            timer.Tick += Timer_Tick;

            _RFIDViewModel.WriteRFIDWindow.RFIDInfoEvent += ReadRFIDWindow_RFIDInfoEvent;
        }
        /// <summary>
        /// 标签初始化
        /// </summary>
        private void TagInitial()
        {
            TagList.GetTag("rfid_sig", out RFID_SigTag, "FxPLC");//RFID读取
            TagList.GetTag("rfid_write", out RFID_WriteTag, "FxPLC");//RFID写入
            if (RFID_SigTag != null)
            {
                RFID_SigTag.PropertyChanged += RFIDWriteTag_PropertyChanged;
            }

            TagList.GetTag("graphic_carton_sn_sig", out SnSig1Tag, "FxPLC");//彩箱SN检测
            TagList.GetTag("product_sn_sig", out SnSig2Tag, "FxPLC");//产品SN检测
            TagList.GetTag("graphic_carton_sn_deal", out SnDeal1Tag, "FxPLC");//彩箱SN处理
            TagList.GetTag("product_sn_deal", out SnDeal2Tag, "FxPLC");//产品SN处理

            if (SnSig1Tag != null)
            {
                SnSig1Tag.PropertyChanged += SnSig1Tag_PropertyChanged; ;
            }
            if (SnSig2Tag != null)
            {
                SnSig2Tag.PropertyChanged += SnSig2Tag_PropertyChanged; ; ;
            }
        }

       

        /// <summary>
        /// 周期刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_taskOrderViewModel.SelectedModel != null)
            {
                //_taskOrderViewModel.SelectedModel.product_count += 1;//读取完成数量
                //_taskOrderViewModel.Update(_taskOrderViewModel.SelectedModel);
            }
        }

        #region RFID处理
        /// <summary>
        /// 写入rfid标签值变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RFIDWriteTag_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Tag<short> tag = sender as Tag<short>;
            if (tag.TagValue == 1 && e.PropertyName== "TagValue")
            {
                _systemLogViewModel.AddMewStatus($"识别到PLC信号{tag.TagAddr}=1，开始写入RFID信息");
                _RFIDViewModel.WriteRFIDWindow.Dispatcher.Invoke(() =>
                {
                    _RFIDViewModel.WriteRFIDWindow.button_reburn_Click(null, null);//调用写入
                });
            }
        }
        /// <summary>
        /// RFID消息处理
        /// </summary>
        /// <param name="PortErrorMessage"></param>
        /// <param name="WriteErrorMessage"></param>
        /// <param name="ReadErrorMessage"></param>
        private void ReadRFIDWindow_RFIDInfoEvent(RfidInfo info)
        {
            if(info.InfoType== RFIDInfoEnum.PortError)
            {
                _systemLogViewModel.AddMewStatus(info.Content, LogTypeEnum.Error);
            }
            else if (info.InfoType == RFIDInfoEnum.WriteError)//写入失败
            {
                _systemLogViewModel.AddMewStatus(info.Content, LogTypeEnum.Error);
                if(RFID_WriteTag !=null && RFID_SigTag.TagValue==1)
                {
                    RFID_WriteTag.Write(2);//失败写入
                }
                if (_taskOrderViewModel.SelectedModel != null)
                {
                    _taskOrderViewModel.SelectedModel.defective_count += 1;//更新异常数量
                    _taskOrderViewModel.Update(_taskOrderViewModel.SelectedModel);
                }
            }
            else if (info.InfoType == RFIDInfoEnum.WriteSuccess)//写入成功,直接显示信息
            {
                _systemLogViewModel.AddMewStatus(info.Content);
            }
            else if (info.InfoType == RFIDInfoEnum.ReadError)//读取失败
            {
                _systemLogViewModel.AddMewStatus(info.Content, LogTypeEnum.Error);
                if(RFID_WriteTag !=null && RFID_SigTag.TagValue == 1)
                {
                    RFID_WriteTag.Write(2);//失败
                }
                if (_taskOrderViewModel.SelectedModel != null)
                {
                    _taskOrderViewModel.SelectedModel.defective_count += 1;//更新异常数量
                    if(_taskOrderViewModel.Update(_taskOrderViewModel.SelectedModel))
                    {
                        _systemLogViewModel.AddMewStatus($"RFID信息读取失败，更新不良品数量，当前不良品数量为{_taskOrderViewModel.SelectedModel.defective_count}", LogTypeEnum.Info);
                    }
                }
            }
            else if (info.InfoType == RFIDInfoEnum.ReadSuccess)//读取成功
            {
                _systemLogViewModel.AddMewStatus(info.Content);
                if (RFID_WriteTag != null && RFID_SigTag.TagValue == 1)
                {
                    RFID_WriteTag.Write(1);//标识复位
                }
            }
            else//其他信息
            {
                _systemLogViewModel.AddMewStatus(info.Content);
            }
        }
        #endregion

        #region 1#产品与2#彩箱sn比对
        private bool Snsig1 = false, Snsig2 = false;
        private void SnSig1Tag_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Tag<short> tag = sender as Tag<short>;
            if (e.PropertyName == "TagValue" && _taskOrderViewModel.SelectedModel != null && _taskOrderViewModel.SelectedModel.enable_check)
            {
                if (tag.TagValue == 1)
                {
                    Snsig1 = true;
                }
                else
                {
                    if(Snsig1)//信号未复位
                    {
                        _systemLogViewModel.AddMewStatus($"产品条码未接收到信号，开始写入失败标识");
                        flagWrite(2);//未查找到条码
                        Snsig1 = false;
                    }
                }
            }
        }
        private void SnSig2Tag_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Tag<short> tag = sender as Tag<short>;
            if(e.PropertyName == "TagValue" && _taskOrderViewModel.SelectedModel != null && _taskOrderViewModel.SelectedModel.enable_check)
            {
                if (tag.TagValue == 1)
                {
                    Snsig2 = true;
                }
                else
                {
                    if (Snsig2)//信号未复位
                    {
                        _systemLogViewModel.AddMewStatus($"彩箱条码未接收到信号，开始写入失败标识");
                        flagWrite(2);//未查找到条码
                        Snsig2 = false;
                    }
                }
            }
        }

        /// <summary>
        /// 初始化相机
        /// </summary>
        /// <param name="liveviewForm"></param>
        /// <param name="index"></param>
        public void InitialCamera(LiveviewForm liveviewForm, int index)
        {
            KeyenceCameraHelper keyenceCameraHelper = new KeyenceCameraHelper(liveviewForm, index);
            keyenceCameraHelper.ConnectToCammera(GlobalData.CameraConfig[index - 1].Ip);
            if (keyenceCameraHelper != null)
            {
                switch (index)
                {
                    case 1:
                        keyenceCameraHelper.NewReaderDataEvent += KeyenceCamera1_NewReaderDataEvent;
                        break;
                    case 2:
                        keyenceCameraHelper.NewReaderDataEvent += KeyenceCamera2_NewReaderDataEvent;
                        break;
                }
            }
            cameraList.Add(keyenceCameraHelper);
        }
        //1#产品条码
        private void KeyenceCamera1_NewReaderDataEvent(string receivedData, int index)
        {
            _systemLogViewModel.AddMewStatus($"{index}相机接收到产品sn为{receivedData}");
            if(!string.IsNullOrEmpty(receivedData.Trim()) && !receivedData.ToLower().Contains("error"))
            {
                Snsig1 = false;
                if (_taskOrderViewModel.SelectedModel != null && _taskOrderViewModel.SelectedModel.enable_check)//允许条码比对
                {
                    if (SnSig1Tag.TagValue == 1)
                    {
                        _taskOrderViewModel.cameraBarcode.product_barcode = receivedData.Trim();
                        StartToCheck();
                    }
                }
            }
        }
        //2#彩箱sn
        private void KeyenceCamera2_NewReaderDataEvent(string receivedData, int index)
        {
            _systemLogViewModel.AddMewStatus($"{index}相机接收到彩箱sn为{receivedData}");
            if (!string.IsNullOrEmpty(receivedData.Trim()) && !receivedData.ToLower().Contains("error"))
            {
                Snsig2 = false;
                if (_taskOrderViewModel.SelectedModel != null && _taskOrderViewModel.SelectedModel.enable_check)//允许条码比对
                {
                    if (SnSig2Tag.TagValue == 1)
                    {
                        _taskOrderViewModel.cameraBarcode.graphic_barcode = receivedData.Trim();
                        StartToCheck();
                    }
                }
            }
        }
        /// <summary>
        /// 任意有条码进入，开始比对。第一次不会成功
        /// 先进入的启动计时，在计时周期以内仍然未成功，写下错误
        /// </summary>
        private void StartToCheck()
        {
            if (_taskOrderViewModel.cameraBarcode.product_barcode == _taskOrderViewModel.cameraBarcode.graphic_barcode)//条码一致
            {
                flagWrite(1);//比对成功1
                _systemLogViewModel.AddMewStatus("标签核对成功，开始复位PLC标识");
            }
            else if(Snsig1 && !Snsig2)//1先到，2还未到，继续等待
            {
                //继续等待
            }
            else if (Snsig2 && !Snsig1)//2先到，1还未到，继续
            {
                //继续等待
            }
        }
        /// <summary>
        /// 反馈标识写入。=1处理成功，条码查找失败=2，比对失败=3
        /// </summary>
        /// <param name="value"></param>
        private void flagWrite(int value)
        {
            if (SnDeal1Tag != null)
            {
                SnDeal1Tag.Write((short)value);
            }
            if (SnDeal2Tag != null)
            {
                SnDeal2Tag.Write((short)value);
            }
        }
        #endregion

        #region 参数下载写入
        /// <summary>
        /// 开始下载参数信息
        /// </summary>
        /// <param name="taskOrder"></param>
        public bool StartToDownloadParamter(TaskOrder taskOrder)
        {
            List<short> ValueList = new List<short>();
            _systemLogViewModel.AddMewStatus($"开始写入{taskOrder.product_name}的PLC参数");
            #region 写入PLC信号
            var plc = TagList.PLCGroups.FirstOrDefault(x => x.PlcDevice.Name == "FxPLC");
            if (plc != null && plc.PlcDevice.IsConnected)
            {
                WritePlcValue("FxPLC", "pack_mode", taskOrder.pack_mode);
                WritePlcValue("FxPLC", "open_machine_mode", taskOrder.open_machine_mode);
                WritePlcValue("FxPLC", "barcode_machine_mode", taskOrder.barcode_machine_mode);
                WritePlcValue("FxPLC", "sn_barcode_enable", -1, taskOrder.sn_barcode_enable);
                WritePlcValue("FxPLC", "bubble_cover_enable", -1, taskOrder.bubble_cover_enable);
                WritePlcValue("FxPLC", "card_machine_enable", -1, taskOrder.card_machine_enable);
            }
            else
            {
                _systemLogViewModel.AddMewStatus("PLC当前未连接，参数写入失败",LogTypeEnum.Error);
                return false;
            }
            #endregion
            _systemLogViewModel.AddMewStatus($"开始写入{taskOrder.product_name}的视觉机械手参数");
            var robot1 = TagList.PLCGroups.FirstOrDefault(x => x.PlcDevice.Name == "Robot1");
            if (robot1 != null && robot1.PlcDevice.IsConnected)
            {
                //写入尺寸信息
                ValueList = new List<short>();
                GetPropertyToList(taskOrder.product_size, ValueList);
                WriteTcpClientRobot("Robot1", "chanpinwaijin", ValueList[0]);
                WriteTcpClientRobot("Robot1", "chanpingaodu", ValueList[1]);
            }
            else
            {
                _systemLogViewModel.AddMewStatus("视觉机械手参数写入失败，请检查网络连接后重新下载！", LogTypeEnum.Error);
                return false;
            }
            _systemLogViewModel.AddMewStatus($"开始写入{taskOrder.product_name}的装箱参数");
            var robot2 = TagList.PLCGroups.FirstOrDefault(x => x.PlcDevice.Name == "Robot2");
            if (robot2 != null && robot2.PlcDevice.IsConnected)
            {
                ValueList = new List<short>();
                GetPropertyToList(taskOrder.product_size, ValueList);
                WriteRobot("Robot2", "product_size_length", ValueList);
            }
            else
            {
                _systemLogViewModel.AddMewStatus("装箱机械手参数写入失败，请检查网络连接后重新下载！", LogTypeEnum.Error);
                return false;
            }
            _systemLogViewModel.AddMewStatus($"开始写入{taskOrder.product_name}的码垛参数");
            var robot3 = TagList.PLCGroups.FirstOrDefault(x => x.PlcDevice.Name == "Robot3");
            if (robot3 != null && robot2.PlcDevice.IsConnected)
            {
                ValueList = new List<short>();
                ValueList.Add(2);
                //ValueList.Add(3);
                //GetPropertyToList(taskOrder.graphic_carton_size, ValueList);
                //GetPropertyToList(taskOrder.noraml_carton_size, ValueList);
                //GetPropertyToList(taskOrder.outer_carton_size, ValueList);
                //GetPropertyToList(taskOrder.pallet_size, ValueList);
                WriteRobot("Robot3", "graphic_carton_size_x", ValueList);//首地址写入
                ValueList = new List<short>();
                ValueList.Add(33);
                WriteRobot("Robot3", "graphic_carton_size_y", ValueList);//首地址写入
                ValueList = new List<short>();
                ValueList.Add((short)taskOrder.robot_pg_no);
                ValueList.Add((short)taskOrder.pallet_num);
                if (taskOrder.plate_enable)
                {
                    ValueList.Add(1);
                }
                else
                {
                    ValueList.Add(0);
                }
                ValueList.Add((short)taskOrder.pack_mode);
               // WriteRobot("Robot3", "robot_pg_no", ValueList);//首地址写入
            }
            else
            {
                _systemLogViewModel.AddMewStatus("码垛机械手参数写入失败，请检查网络连接后重新下载！", LogTypeEnum.Error);
                return false;
            }
            _systemLogViewModel.AddMewStatus($"{taskOrder.product_name}所有参数已下载完毕");
            return true;
        }
        private void WritePlcValue(string plcName, string TagName,int value=-1,bool boolValue=false)
        {
            Tag<short> tag;
            TagList.GetTag(TagName, out tag, plcName);
            if (tag != null)
            {
                if(value>0)
                {
                    tag.Write((short)value);
                }
                else if(boolValue)
                {
                    tag.Write(1);
                }
                else
                {
                    tag.Write(0);
                }
            }
        }
        /// <summary>
        /// 写入尺寸信息
        /// </summary>
        /// <param name="RobotName"></param>
        /// <param name="TagName"></param>
        /// <param name="size"></param>
        private void WriteTcpClientRobot(string RobotName, string TagName, int tagValue)
        {
            var plc = TagList.PLCGroups.FirstOrDefault(x => x.PlcDevice.Name == RobotName);
            if (plc != null)
            {
                TcpServer tcpServer = plc.plcDriverHelper as TcpServer;
                if (tcpServer != null)
                {
                    tcpServer.SendMessageToClient($"[CamSetVar(\"{TagName}\",{tagValue},1);id=112]");
                    _systemLogViewModel.AddMewStatus($"写入{RobotName}地址{TagName}成功，写入值为{tagValue}", LogTypeEnum.Info);
                }
            }
        }
        /// <summary>
        /// 写入尺寸信息
        /// </summary>
        /// <param name="RobotName"></param>
        /// <param name="TagName"></param>
        /// <param name="size"></param>
        private bool WriteRobot(string RobotName, string TagName, List<short> ValueList)
        {
            int count = ValueList.Count;
            short[] value = new short[count];
            Tag<short> tag;
            TagList.GetTag(TagName, out tag, RobotName);

            if (tag != null)
            {
                for (int i = 0; i < ValueList.Count; i++)
                {
                    value[i] = ValueList[i];
                }

                var plc = TagList.PLCGroups.FirstOrDefault(x => x.PlcDevice.Name == RobotName);
                if (plc != null)
                {
                    if (plc.plcDriverHelper.BatchWriteValue(tag.TagAddr, value))
                    {
                        _systemLogViewModel.AddMewStatus($"写入{RobotName}地址{tag.TagAddr}成功，写入数量为{ValueList.Count}", LogTypeEnum.Info);
                        return true;
                    }
                    else
                    {
                        _systemLogViewModel.AddMewStatus($"写入{RobotName}地址{tag.TagAddr}失败，写入数量为{ValueList.Count}", LogTypeEnum.Error);
                    }
                }
            }
            return false;
        }

        public bool GetPropertyToList(string setValue,List<short> ValueList)
        {
            short value = 0;
            string[] valeus = setValue.Split('*');
            if (valeus.Length > 0)
            {
                for (int i = 0; i < valeus.Length; i++)
                {
                    short.TryParse(valeus[i], out value);
                    ValueList.Add(value);
                }
                return true;
            }

            return false;
        }

        public void Dispose()
        {
            if(cameraList !=null)
            {
                foreach (var item in cameraList)
                {
                    item.Dispose();
                }
            }
        }
        #endregion


    }
}
