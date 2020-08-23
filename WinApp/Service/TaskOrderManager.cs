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
using System.Text;
using System.Threading.Tasks;
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
        //队列1
        private void KeyenceCamera1_NewReaderDataEvent(string receivedData, int index)
        {
            lastBarcode1 = receivedData;
            _taskOrderViewModel.cameraBarcode.product_barcode = lastBarcode1;
            _systemLogViewModel.AddMewStatus($"接收到产品sn为{lastBarcode1}");
        }
        //队列2
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
                if (lastBarcode1 == lastBarcode2)//已经在计时，进行比较
                {
                    lastBarcode1 = string.Empty;
                    lastBarcode2 = string.Empty;
                    TagList.GetTag("graphic_sn", out tag1);
                    TagList.GetTag("product_sn", out tag2);
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
            }
            if (!barcodeCheckTimer.IsEnabled)//未开始计时
            {
                barcodeCheckTimer.Start();
            }
        }
        private void barcodeCheckTimer_Tick(object sender, EventArgs e)//时间到达
        {
            Tag<short> tag1, tag2;
            TagList.GetTag("graphic_sn", out tag1);
            TagList.GetTag("product_sn", out tag2);
            barcodeCheckTimer.Stop();
            if (!string.IsNullOrEmpty(lastBarcode1) && !string.IsNullOrEmpty(lastBarcode2))
            {
                if (lastBarcode1 == lastBarcode2)//已经在计时，进行比较
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
        /// <summary>
        /// 开始下载参数信息
        /// </summary>
        /// <param name="taskOrder"></param>
        public void StartToDownloadParamter(TaskOrder taskOrder)
        {
            Tag<short> tag;
            short value = 0;
            #region 写入PLC信号
            TagList.GetTag("pack_mode", out tag);
            if(tag !=null)
            {
                tag.Write((short)taskOrder.pack_mode);
            }
            TagList.GetTag("open_machine_mode", out tag);
            if (tag != null)
            {
                tag.Write((short)taskOrder.open_machine_mode);
            }
            TagList.GetTag("barcode_machine_mode", out tag);
            if (tag != null)
            {
                tag.Write((short)taskOrder.barcode_machine_mode);
            }
            TagList.GetTag("sn_barcode_enable", out tag);
            if (tag != null)
            {
                value = taskOrder.sn_barcode_enable == true ? (short)1 : (short)0;
                tag.Write(value);
            }
            TagList.GetTag("card_machine_enable", out tag);
            if (tag != null)
            {
                value = taskOrder.card_machine_enable == true ? (short)1 : (short)0;
                tag.Write(value);
            }
            TagList.GetTag("plate_enable", out tag);
            if (tag != null)
            {
                value = taskOrder.plate_enable == true ? (short)1 : (short)0;
                tag.Write(value);
            }
            #endregion

            //写入机械手信号
        }
    }
}
