using iFactory.CommonLibrary;
using iFactory.DataService.IService;
using iFactory.DataService.Model;
using iFactory.DevComServer;
using iFactoryApp.Common;
using iFactoryApp.ViewModel;
using Keyence.AutoID.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;

namespace iFactoryApp.Service
{
    public class TaskOrderManager
    {
        private readonly ITaskOrderService _taskOrderService;
        private readonly ITaskOrderHistoryService _taskOrderHistoryService;
        private readonly ITaskOrderDetailService _taskOrderDetailService;
        private readonly ITaskOrderDetailHistoryService _taskOrderDetailHistoryService;
        private readonly IProductParameterService _productParameterService;
        private readonly ISystemLogViewModel _systemLogViewModel;
        private readonly TaskOrderViewModel _taskOrderViewModel;
        public List<KeyenceCameraHelper> cameraList = new List<KeyenceCameraHelper>();

        private DispatcherTimer barcodeCheckTimer=new DispatcherTimer();
        private string lastBarcode1, lastBarcode2;

        public TaskOrderManager(ITaskOrderService taskOrderService,
                                ITaskOrderHistoryService taskOrderHistoryService,
                                ITaskOrderDetailService taskOrderDetailService,
                                ITaskOrderDetailHistoryService taskOrderDetailHistoryService,
                                IProductParameterService productParameterService,
                                ISystemLogViewModel systemLogViewModel,
                                TaskOrderViewModel taskOrderViewModel)
        {
            _taskOrderService = taskOrderService;
            _taskOrderDetailService = taskOrderDetailService;
            _taskOrderHistoryService = taskOrderHistoryService;
            _taskOrderDetailHistoryService = taskOrderDetailHistoryService;
            _productParameterService = productParameterService;
            _systemLogViewModel = systemLogViewModel;
            _taskOrderViewModel = taskOrderViewModel;

            barcodeCheckTimer.Interval = TimeSpan.FromSeconds(5);//5秒
            barcodeCheckTimer.Tick += barcodeCheckTimer_Tick;
        }
        private void InitialRfid()
        {
            Tag<short> tag;
            TagList.GetTag("rfid_write", out tag, "FxPLC");
            if(tag !=null)
            {
                tag.PropertyChanged += Tag_PropertyChanged;
            }
        }

        //写入rfid标签值变化
        private void Tag_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            
        }

        /// <summary>
        /// 初始化相机
        /// </summary>
        /// <param name="liveviewForm"></param>
        /// <param name="index"></param>
        public void InitialCamera(LiveviewForm liveviewForm, int index)
        {
            KeyenceCameraHelper keyenceCameraHelper = new KeyenceCameraHelper(liveviewForm, index);
            keyenceCameraHelper.ConnectToCammera(GlobalSettings.CameraConfig[index - 1].Ip);
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

        #region 产品与彩箱sn比对
        //产品条码
        private void KeyenceCamera1_NewReaderDataEvent(string receivedData, int index)
        {
            lastBarcode1 = receivedData;
            _taskOrderViewModel.cameraBarcode.product_barcode = lastBarcode1;
            _systemLogViewModel.AddMewStatus($"接收到产品sn为{lastBarcode1}");
        }
        //彩箱sn
        private void KeyenceCamera2_NewReaderDataEvent(string receivedData, int index)
        {
            lastBarcode2 = receivedData;
            _taskOrderViewModel.cameraBarcode.graphic_barcode = lastBarcode2;
            _systemLogViewModel.AddMewStatus($"接收到彩箱sn为{lastBarcode2}");
        }
        private void StartTimer()
        {
            Tag<short> tag1, tag2;
            if (!string.IsNullOrEmpty(lastBarcode1) && !string.IsNullOrEmpty(lastBarcode2))
            {
                TagList.GetTag("graphic_sn", out tag1, "FxPLC");
                TagList.GetTag("product_sn", out tag2, "FxPLC");

                if (lastBarcode1 == lastBarcode2)//条码一致
                {
                    lastBarcode1 = string.Empty;
                    lastBarcode2 = string.Empty;
                   
                    if(tag1 !=null)
                    {
                        tag1.Write(0);
                    }
                    if (tag2 != null)
                    {
                        tag2.Write(0);
                    }
                    _systemLogViewModel.AddMewStatus("sn标签核对成功，复位PLC标识");
                    return;
                }
                else
                {
                    lastBarcode1 = string.Empty;
                    lastBarcode2 = string.Empty;

                    if (tag1 != null)
                    {
                        tag1.Write(2);//对比失败
                    }
                    if (tag2 != null)
                    {
                        tag2.Write(2);//对比失败
                    }
                    _systemLogViewModel.AddMewStatus("sn标签核对失败");
                    return;
                }
            }

            if (!barcodeCheckTimer.IsEnabled)//未开始计时
            {
                barcodeCheckTimer.Start();
            }
        }
        private void barcodeCheckTimer_Tick(object sender, EventArgs e)//时间到达
        {
            Tag<short> tag1, tag2;
            TagList.GetTag("graphic_sn", out tag1, "FxPLC");
            TagList.GetTag("product_sn", out tag2, "FxPLC");
            barcodeCheckTimer.Stop();
            if (!string.IsNullOrEmpty(lastBarcode1) && !string.IsNullOrEmpty(lastBarcode2))
            {
                if (lastBarcode1 == lastBarcode2)//进行比较
                {
                    lastBarcode1 = string.Empty;
                    lastBarcode2 = string.Empty;
                    if (tag1 != null)
                    {
                        tag1.Write(0);
                    }
                    if (tag2 != null)
                    {
                        tag2.Write(0);
                    }
                    _systemLogViewModel.AddMewStatus("时间到达，sn标签核对成功，复位PLC标识");
                    return;
                }
            }

            _systemLogViewModel.AddMewStatus("sn标签核对超时错误，写入PLC错误信息", LogTypeEnum.Error);
            if (tag1 != null)
            {
                tag1.Write(2);//写入错误
            }
            if (tag2 != null)
            {
                tag2.Write(2);//写入错误
            }
        }
        #endregion

        /// <summary>
        /// 开始下载参数信息
        /// </summary>
        /// <param name="taskOrder"></param>
        public bool StartToDownloadParamter(TaskOrder taskOrder)
        {
            List<short> ValueList = new List<short>();
            _systemLogViewModel.AddMewStatus("开始写入PLC参数");
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
                _systemLogViewModel.AddMewStatus("PLC参数写入失败",LogTypeEnum.Error);
                return false;
            }
            #endregion
            var robot1 = TagList.PLCGroups.FirstOrDefault(x => x.PlcDevice.Name == "Robot1");
            if (robot1 != null && robot1.PlcDevice.IsConnected)
            {
                //写入尺寸信息
                ValueList = new List<short>();
                GetPropertyToList(taskOrder.product_size, ValueList);
                WriteRobot("Robot1", "product_size", ValueList);
            }
            else
            {
                _systemLogViewModel.AddMewStatus("Robot1机械手参数写入失败", LogTypeEnum.Error);
                return false;
            }

            var robot2 = TagList.PLCGroups.FirstOrDefault(x => x.PlcDevice.Name == "Robot2");
            if (robot2 != null && robot2.PlcDevice.IsConnected)
            {
                ValueList = new List<short>();
                GetPropertyToList(taskOrder.product_size, ValueList);
                WriteRobot("Robot2", "product_size", ValueList);
            }
            else
            {
                _systemLogViewModel.AddMewStatus("Robot2机械手参数写入失败", LogTypeEnum.Error);
                return false;
            }

            var robot3 = TagList.PLCGroups.FirstOrDefault(x => x.PlcDevice.Name == "Robot3");
            if (robot3 != null && robot2.PlcDevice.IsConnected)
            {
                ValueList = new List<short>();
                GetPropertyToList(taskOrder.graphic_carton_size, ValueList);
                GetPropertyToList(taskOrder.noraml_carton_size, ValueList);
                GetPropertyToList(taskOrder.outer_carton_size, ValueList);
                GetPropertyToList(taskOrder.pallet_size, ValueList);
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
                WriteRobot("Robot3", "graphic_carton_size", ValueList);//首地址写入
            }
            else
            {
                _systemLogViewModel.AddMewStatus("Robot3机械手参数写入失败", LogTypeEnum.Error);
                return false;
            }
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
        private void WriteRobot(string RobotName, string TagName, List<short> ValueList)
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
                    }
                    else
                    {
                        _systemLogViewModel.AddMewStatus($"写入{RobotName}地址{tag.TagAddr}失败，写入数量为{ValueList.Count}", LogTypeEnum.Error);
                    }
                }
            }
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
    }
}
