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
        private Tag<short> RFID_WriteSigTag, RFID_WriteFeedbackTag;
        private Tag<short> RFID_ReadSigTag, RFID_ReadFeedbackTag;
        private Tag<short> SnSig1Tag, SnSig2Tag, SnDeal1Tag, SnDeal2Tag;
        private readonly DispatcherTimer timer;
        private readonly DispatcherTimer rfidReadTimer;

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

            rfidReadTimer = new DispatcherTimer() { IsEnabled = false, Interval = TimeSpan.FromSeconds(5) };//5秒检测是否写入成功
            rfidReadTimer.Tick += RfidReadTimer_Tick; ;

            _RFIDViewModel.WriteRFIDWindow.RFIDInfoEvent += ReadRFIDWindow_RFIDInfoEvent;
            _RFIDViewModel.ReadRFIDWindow.RFIDInfoEvent += ReadRFIDWindow_RFIDInfoEvent;
        }
        /// <summary>
        /// 标签初始化
        /// </summary>
        private void TagInitial()
        {
            TagList.GetTag("rfid_write_sig", out RFID_WriteSigTag, "FxPLC");//RFID触发信号
            TagList.GetTag("rfid_write_feedback", out RFID_WriteFeedbackTag, "FxPLC");//RFID写入
            TagList.GetTag("rfid_read_sig", out RFID_ReadSigTag, "FxPLC");//RFID触发信号
            TagList.GetTag("rfid_read_feedback", out RFID_ReadFeedbackTag, "FxPLC");//RFID写入

            if (RFID_WriteSigTag != null)
            {
                RFID_WriteSigTag.PropertyChanged += RFIDWriteTag_PropertyChanged;
            }

            if (RFID_ReadSigTag != null)
            {
                RFID_ReadSigTag.PropertyChanged += RFID_ReadSigTag_PropertyChanged; ;
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
        //
        #region RFID处理
        private string lastWriteInfo = string.Empty;
        private void RFID_ReadSigTag_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Tag<short> tag = sender as Tag<short>;
            if (tag.TagValue == 1 && e.PropertyName == "TagValue")
            {
                _systemLogViewModel.AddMewStatus($"识别到PLC信号{tag.TagAddr}=1，开始读取RFID信息");
                _RFIDViewModel.ReadRFIDWindow.Dispatcher.Invoke(() =>
                {
                    _RFIDViewModel.ReadRFIDWindow.button_read_filter_data_Click(null, null);
                    if (!_RFIDViewModel.WriteRFIDWindow.IsReadSuccess)//读取成功
                    {
                       
                    }
                });
            }
        }
        private void RfidReadTimer_Tick(object sender, EventArgs e)
        {
            //////rfidReadTimer.IsEnabled = false;
            //////rfidReadTimer.Stop();
            //////if (RFID_ReadSigTag != null && RFID_ReadSigTag.TagValue==1)
            //////{
            //////    RFID_ReadFeedbackTag.Write(2);//超时，写入PLC失败=2
            //////}
            //////_systemLogViewModel.AddMewStatus($"RFID读取超时，开始写入PLC=2失败");
        }
        /// <summary>
        /// 写入rfid标签值变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RFIDWriteTag_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Tag<short> tag = sender as Tag<short>;
            if (tag.TagValue == 1 && e.PropertyName == "TagValue")
            {
                _systemLogViewModel.AddMewStatus($"识别到PLC信号{tag.TagAddr}=1，开始写入RFID信息");
                //Thread.Sleep(5000);
                _RFIDViewModel.WriteRFIDWindow.Dispatcher.Invoke(() =>
                {
                    _RFIDViewModel.WriteRFIDWindow.button_burn_Click(null, null);
                    if (!_RFIDViewModel.WriteRFIDWindow.IsBurnSuccess)//烧写未成功，调用重新烧录
                    {
                        _systemLogViewModel.AddMewStatus($"RFID写入未成功，开始调用重新烧录RFID信息");
                        _RFIDViewModel.WriteRFIDWindow.button_reburn_Click(null, null);//调用写入
                    }
                    short value = 1;
                    if (!_RFIDViewModel.WriteRFIDWindow.IsBurnSuccess)//写入失败
                    {
                        value = 2;
                    }
                    if (RFID_WriteFeedbackTag != null && RFID_WriteSigTag.TagValue == 1)
                    {
                        RFID_WriteFeedbackTag.Write(value);//写入PLC信号
                    }
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
               
                if (_taskOrderViewModel.SelectedModel != null)
                {
                    _taskOrderViewModel.SelectedModel.defective_count += 1;//更新异常数量
                    _taskOrderViewModel.Update(_taskOrderViewModel.SelectedModel);
                }
            }
            else if (info.InfoType == RFIDInfoEnum.WriteSuccess)//写入成功,直接显示信息
            {
                lastWriteInfo = info.Sn;
                _systemLogViewModel.AddMewStatus(info.Content);
            }
            else if (info.InfoType == RFIDInfoEnum.ReadError)//读取失败
            {
                //rfidReadTimer.IsEnabled = false;
                //rfidReadTimer.Stop();
                _systemLogViewModel.AddMewStatus(info.Content, LogTypeEnum.Error);
                if(RFID_ReadFeedbackTag !=null && RFID_ReadSigTag.TagValue == 1)
                {
                    RFID_ReadFeedbackTag.Write(2);
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
                //rfidReadTimer.IsEnabled = false;
                //rfidReadTimer.Stop();
                _systemLogViewModel.AddMewStatus(info.Content);
                if (info.Sn== lastWriteInfo)
                {
                    _systemLogViewModel.AddMewStatus("RFID比对成功！开始写入PLC=1");
                    if (RFID_ReadFeedbackTag != null && RFID_ReadSigTag.TagValue == 1)
                    {
                        RFID_ReadFeedbackTag.Write(1);//
                    }
                }
                else
                {
                    _systemLogViewModel.AddMewStatus("RFID比对失败！开始写入PLC=2");
                    if (RFID_ReadFeedbackTag != null && RFID_ReadSigTag.TagValue == 1)
                    {
                        RFID_ReadFeedbackTag.Write(2);
                    }
                }
            }
        }
        #endregion
        #region 1#产品与2#彩箱sn检测与比对
        /// <summary>
        /// 信号1复位
        /// </summary>
        public void RstSnSig1()
        {
            _systemLogViewModel.AddMewStatus($"开始复位产品相机的信号");
            Snsig1 = false;
            PreSn1Barcode = "";
            _taskOrderViewModel.cameraBarcode.product_barcode = "";
            flagWrite(0, sn1: true, sn2: false);//信号复位
            if (SnDeal2Tag.TagValue == 4)//对比失败
            {
                _systemLogViewModel.AddNewAutoAckWindowInfo($"当前彩箱信号为对比失败，将彩箱信号=1",null,0);
                flagWrite(1, sn1: false, sn2: true);//信号2复位
            }
        }
        /// <summary>
        /// 信号2复位
        /// </summary>
        public void RstSnSig2()
        {
            _systemLogViewModel.AddMewStatus($"开始复位彩箱相机的信号");
            Snsig2 = false;
            PreSn2Barcode = "";
            _taskOrderViewModel.cameraBarcode.graphic_barcode = "";
            flagWrite(0, sn1: false, sn2: true);//信号2复位
            if (SnDeal1Tag.TagValue == 4)//对比失败
            {
                _systemLogViewModel.AddNewAutoAckWindowInfo($"当前产品信号为对比失败，将产品信号=1",null,0);
                flagWrite(1, sn1: true, sn2: false);//信号1复位
            }
        }
        private bool Snsig1 = false, Snsig2 = false;
        private string PreSn1Barcode = string.Empty, PreSn2Barcode = string.Empty;//当前条码
        private void SnSig1Tag_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Tag<short> tag = sender as Tag<short>;
            bool enable = false;
            if (_taskOrderViewModel.SelectedModel != null)
            {
                enable = _taskOrderViewModel.SelectedModel.enable_check || _taskOrderViewModel.SelectedModel.sn_barcode_enable;
            }
            if (e.PropertyName == "TagValue" && enable)
            {
                if (tag.TagValue == 1)
                {
                    _systemLogViewModel.AddMewStatus($"检测到产品相机的PLC信号=1");
                    PreSn1Barcode = "";
                    Snsig1 = true;
                }
                else
                {
                    if (Snsig1)//信号未复位
                    {
                        _systemLogViewModel.AddNewAutoAckWindowInfo($"产品相机未检测到条码，当前标签值={tag.TagValue},开始写入PLC失败信号值=3",null,0);
                        flagWrite(3, sn1: true, sn2: false);//未查找到条码
                        Snsig1 = false;
                    }
                }
            }
        }
        private void SnSig2Tag_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Tag<short> tag = sender as Tag<short>;
            bool enable = false;
            if (_taskOrderViewModel.SelectedModel != null)
            {
                enable = _taskOrderViewModel.SelectedModel.enable_check || _taskOrderViewModel.SelectedModel.sn_barcode_enable;
            }
            if (e.PropertyName == "TagValue" && enable)
            {
                if (tag.TagValue == 1)
                {
                    _systemLogViewModel.AddMewStatus($"检测到彩箱相机的PLC信号=1");
                    PreSn2Barcode = "";
                    Snsig2 = true;
                }
                else
                {
                    if (Snsig2)//信号未复位
                    {
                        _systemLogViewModel.AddNewAutoAckWindowInfo($"彩箱相机未检测到条码，当前标签值={tag.TagValue},开始写入PLC失败信号值=3",null,0);
                        flagWrite(3, sn1: false, sn2: true);//未查找到条码
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
            _systemLogViewModel.AddMewStatus($"产品相机检测到条码信息为：{receivedData}");
            if (!string.IsNullOrEmpty(receivedData.Trim()) && !receivedData.ToLower().Contains("error"))
            {
                PreSn1Barcode = receivedData.Trim();
                _taskOrderViewModel.cameraBarcode.product_barcode = PreSn1Barcode;
                Snsig1 = false;//复位存在标识
                flagWrite(1, sn1: true, sn2: false);//查找到条码
                if (_taskOrderViewModel.SelectedModel != null && _taskOrderViewModel.SelectedModel.enable_check)//允许条码比对
                {
                    if (SnSig1Tag.TagValue == 1)
                    {
                        _systemLogViewModel.AddMewStatus($"产品相机触发信号值=1，开始进行条码对比");
                        StartToCheck();
                    }
                }
            }
        }
        //2#彩箱sn
        private void KeyenceCamera2_NewReaderDataEvent(string receivedData, int index)
        {
            _systemLogViewModel.AddMewStatus($"彩箱相机检测到条码为：{receivedData}");
            if (!string.IsNullOrEmpty(receivedData.Trim()) && !receivedData.ToLower().Contains("error"))
            {
                PreSn2Barcode = receivedData.Trim();
                _taskOrderViewModel.cameraBarcode.graphic_barcode = PreSn2Barcode;
                Snsig2 = false;//复位存在标识
                flagWrite(1, sn1: false, sn2: true);//查找到条码
                if (_taskOrderViewModel.SelectedModel != null && _taskOrderViewModel.SelectedModel.enable_check)//允许条码比对
                {
                    if (SnSig2Tag.TagValue == 1)
                    {
                        _systemLogViewModel.AddMewStatus($"彩箱相机触发信号值=1，开始进行条码对比");
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
            if (PreSn1Barcode.Length > 0 && PreSn2Barcode.Length > 0)
            {
                if (_taskOrderViewModel.cameraBarcode.product_barcode == _taskOrderViewModel.cameraBarcode.graphic_barcode)//条码一致
                {
                    flagWrite(2);//比对成功2
                    _systemLogViewModel.AddMewStatus("标签对比成功，写入PLC值=2，开始复位PLC标识");
                    PreSn1Barcode = "";
                    PreSn2Barcode = "";
                }
                else
                {
                    flagWrite(4);
                    _systemLogViewModel.AddNewAutoAckWindowInfo("标签对比失败，写入PLC值=4，开始复位PLC标识",null,0);
                }
            }
            else
            {
                if (PreSn1Barcode.Length == 0)
                {
                    _systemLogViewModel.AddMewStatus("当前不满足对比条件，因为产品的条码信息不具备");
                }
                else
                {
                    _systemLogViewModel.AddMewStatus("当前不满足对比条件，因为彩箱条码信息不具备");
                }
            }
        }
        /// <summary>
        /// 反馈标识写入。=1条码检测成功，条码检测失败=3，条码比对成功=2，对比失败写4
        /// </summary>
        /// <param name="value"></param>
        private void flagWrite(int value, bool sn1 = true, bool sn2 = true)
        {
            if (SnDeal1Tag != null && sn1)
            {
                SnDeal1Tag.Write((short)value);
            }
            if (SnDeal2Tag != null && sn2)
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
                WritePlcValue("FxPLC", "sn_barcode_compare", -1, taskOrder.enable_check);//条码比对
            }
            else
            {
                _systemLogViewModel.AddMewStatus("PLC当前未连接，参数写入失败", LogTypeEnum.Error);
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
            //_systemLogViewModel.AddMewStatus($"开始写入{taskOrder.product_name}的码垛参数");
            //var robot3 = TagList.PLCGroups.FirstOrDefault(x => x.PlcDevice.Name == "Robot3");
            //if (robot3 != null && robot2.PlcDevice.IsConnected)
            //{
            //    ValueList = new List<short>();
            //    ValueList.Add(2);
            //    //ValueList.Add(3);
            //    //GetPropertyToList(taskOrder.graphic_carton_size, ValueList);
            //    //GetPropertyToList(taskOrder.noraml_carton_size, ValueList);
            //    //GetPropertyToList(taskOrder.outer_carton_size, ValueList);
            //    //GetPropertyToList(taskOrder.pallet_size, ValueList);
            //    WriteRobot("Robot3", "graphic_carton_size_x", ValueList);//首地址写入
            //    ValueList = new List<short>();
            //    ValueList.Add(33);
            //    WriteRobot("Robot3", "graphic_carton_size_y", ValueList);//首地址写入
            //    ValueList = new List<short>();
            //    ValueList.Add((short)taskOrder.robot_pg_no);
            //    ValueList.Add((short)taskOrder.pallet_num);
            //    if (taskOrder.plate_enable)
            //    {
            //        ValueList.Add(1);
            //    }
            //    else
            //    {
            //        ValueList.Add(0);
            //    }
            //    ValueList.Add((short)taskOrder.pack_mode);
            //    // WriteRobot("Robot3", "robot_pg_no", ValueList);//首地址写入
            //}
            //else
            //{
            //    _systemLogViewModel.AddMewStatus("码垛机械手参数写入失败，请检查网络连接后重新下载！", LogTypeEnum.Error);
            //    return false;
            //}
            _systemLogViewModel.AddNewAutoAckWindowInfo($"{taskOrder.product_name}所有参数已下载完毕",null,0);
            return true;
        }
        private void WritePlcValue(string plcName, string TagName, int value = -1, bool boolValue = false)
        {
            Tag<short> tag;
            TagList.GetTag(TagName, out tag, plcName);
            if (tag != null)
            {
                if (value > 0)
                {
                    tag.Write((short)value);
                }
                else if (boolValue)
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

        public bool GetPropertyToList(string setValue, List<short> ValueList)
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
            if (cameraList != null)
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
