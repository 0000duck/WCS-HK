using System.Windows;
using System.IO.Ports;
using System.IO;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Collections;
using System.Threading;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Controls;

namespace RFIDLib
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RFIDWindow : Page
    {
        string lastWriteInfo = string.Empty;//上一次写入

        SerialPortUtils serialPortUtils = new SerialPortUtils();
        SerialPort serialPort = new SerialPort();//当前选择的串口
        string recieveData = "";
        LogHandleUtils logHandleUtils = new LogHandleUtils();
        DataHandleUtils dataHandleUtils = new DataHandleUtils();
        ConfigHandleUtils configHandleUtils = new ConfigHandleUtils();
        DataUtils dataUtils = new DataUtils();
        SoundPlay soundPlay = new SoundPlay();
        System.Windows.Forms.Timer readTimer = new System.Windows.Forms.Timer();
        string log;
        int tryTimes = 8;
        int sleepTime = 100;

        string factory_num;
        string product_num;
        string product_date;
        string serial_num;
        string work_command_num;
        int serial_num_int;
        bool bReadSuccessiveFlag = false;

        private char[] HexChar = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

        public RFIDWindow(string ComPortName)
        {
            
            InitializeComponent();
            
            logHandleUtils.createLogPath();
            dataHandleUtils.createDataPath();
            configHandleUtils.createConfigPath();

            //Check serial ports
            string[] portList = serialPortUtils.getPortNameList();
            textBoxCom.Text = ComPortName;

            serialPort = serialPortUtils.OpenPort(ComPortName, logHandleUtils);
        
            if (serialPort.IsOpen)
            {
                textBox_info_box.Text = ComPortName + "已打开";
                logHandleUtils.writeLog(ComPortName + "已打开");
            }
            else
            {
                textBox_info_box.Text = ComPortName + "打开失败";
                logHandleUtils.writeLog(ComPortName + "打开失败");
                RfidInfo info = new RfidInfo() { InfoType = RFIDInfoEnum.PortError, Content = $"RFID端口{ComPortName}打开失败" };
                SendRfidOperateEvent(info);
            }

            string configPath = configHandleUtils.getConfigFilePath();
            string configString = File.ReadAllText(configPath, Encoding.UTF8);
            if (1 < configString.Length)
            {
                string[] configArray = configString.Split('-');
                textBox_factory_NO.Text = configArray[0];
                textBox_product_NO.Text = configArray[1];
                textBox_serial_NO.Text = configArray[2];
                Check_WorkCommand.IsChecked = bool.Parse(configArray[3]);
                if (Check_WorkCommand.IsChecked == true)
                {
                    textBox_work_command_NO.IsEnabled = true;
                    textBox_work_command_NO.Text = configArray[4];
                    work_command_num = textBox_work_command_NO.Text;
                }

                factory_num = textBox_factory_NO.Text;
                product_num = textBox_product_NO.Text;
                serial_num = textBox_serial_NO.Text;
            }

            DateTime dt = DateTime.Now;
            string monthDay = dt.ToString("MMdd");
            string year = dt.ToString("yyyy").Substring(2);
            textBox_product_date.Text = year + monthDay;

            readTimer.Interval = 200;
            readTimer.Enabled = false;
            readTimer.Tick += new EventHandler(Timer_TimesUp);
        }

        //连续读取的定时器
        private void Timer_TimesUp(object sender, EventArgs e)
        {
            recieveData = "";
            string showResult = "";
            string result = getFilterData();

            string[] data = dataUtils.getRecieveFilterData(recieveData);
            int filterCode = dataUtils.getResultCode(recieveData);

            if (filterCode == 84)
            {
                showResult = result + "\n" + dataUtils.showFilterData(data);
            }

            result = getFilterSN();
            string[] sn = dataUtils.getRecieveSN(recieveData);
            int snCode = dataUtils.getResultCode(recieveData);

            if (snCode == 85)
            {
                showResult += "\n" + "sn:" + sn[0] + "\n" + "滤芯使用时长(秒):" + sn[1];
            }

            if ((filterCode == 84) && (snCode == 85))
            {
                textBox_info_box.Background = new SolidColorBrush(Constans.successColor);
                textBox_info_box.Text = showResult;
                logHandleUtils.writeLog(textBox_info_box.Text);
                soundPlay.playSuccess();

                try
                {
                    if (lastWriteInfo != sn[0])
                    {
                        logHandleUtils.writeLog($"读取到的为：{sn[0]}，上一个为：{lastWriteInfo}，比对失败");
                        RfidInfo info = new RfidInfo() { InfoType = RFIDInfoEnum.ReadError, Content = $"读取到的为：{sn[0]}，上一个为：{lastWriteInfo}，比对失败" };
                        SendRfidOperateEvent(info);
                    }
                    else
                    {
                        RfidInfo info = new RfidInfo() { InfoType = RFIDInfoEnum.ReadSuccess, Content = $"读取到的为：{sn[0]}，上一个为：{lastWriteInfo}，比对成功" };
                        SendRfidOperateEvent(info);
                    }
                }
                catch (Exception ex)
                {
                    logHandleUtils.writeLog($"连续读取错误:{ex.Message}");
                    RfidInfo info = new RfidInfo() { InfoType = RFIDInfoEnum.ReadError, Content = $"连续读取错误:{ex.Message}" };
                    SendRfidOperateEvent(info);
                }
            }
        }
        private void button_sp_test_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                getFilterData();
                string[] result = dataUtils.getRecieveFilterData(recieveData);
                string str = "";
                for (int i = 0; i < result.Length; i++)
                {
                    str += result[i];
                }
                Thread.Sleep(500);
                getSN();

                //interfaceUpdateHandle2= new HandleInterfaceUpdateDelegate(getSN);
                // this.Dispatcher.Invoke(interfaceUpdateHandle2, null);
                if ((str + recieveData) != "")
                {
                    textBox_info_box.Text = str + "\n" + recieveData;
                }

                //textBox_info_box.Text= Encoding.Default.GetString(bytes);

            }
            catch (Exception ex)
            {
                textBox_info_box.Text = "串口写入错误。" + ex.Message;
                logHandleUtils.writeLog("串口写入错误。" + ex.Message);
            }
        }

        private void textBox_factory_NO_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            factory_num = textBox_factory_NO.Text;
        }

        private void textBox_product_NO_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            product_num = textBox_product_NO.Text;
        }

        private void textBox_product_date_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            product_date = textBox_product_date.Text;
        }

        private void textBox_work_command_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            work_command_num = textBox_work_command_NO.Text;
        }

        /// <summary>
        /// 烧录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void button_burn_Click(object sender, RoutedEventArgs e)
        {
            lastWriteInfo = string.Empty;
            textBox_info_box.Background = new SolidColorBrush(Constans.whtieColor);
            recieveData = "";
            if (serialPort != null && serialPort.IsOpen)
            {
                //if (Regex.IsMatch(textBox_serial_NO.Text, @"^[+-]?\d*[.]?\d*$"))
                if (true)
                {
                    string result = burnFilterData(factory_num, product_num, product_date, textBox_serial_NO.Text);
                    int code = dataUtils.getResultCode(recieveData);
                    //Thread.Sleep(200);
                    if (code == 80)
                    {
                        soundPlay.playSuccess();
                        serial_num_int = int.Parse(serial_num);
                        result = getFilterSN();
                        string[] sn = dataUtils.getRecieveSN(recieveData);
                        //textBox_info_box.Text = result + "\n" + "sn:" + sn[0] + "\n" + "滤芯使用时长(秒):" + sn[1];
                        textBox_info_box.Text = "烧写成功！\n" + "sn:" + sn[0] + "\n" + "滤芯使用时长(秒):" + sn[1];
                        lastWriteInfo = sn[0];//烧写信息加入队列
                        textBox_info_box.Background = new SolidColorBrush(Constans.successColor);
                        string data = factory_num + "\t" + product_num + "\t" + textBox_serial_NO.Text + "\t" + product_date + "\t" + sn[0];
                        if (Check_WorkCommand.IsChecked == true)
                        {
                            data += "\t" + work_command_num;
                        }
                        dataHandleUtils.writeData(data);

                        log = "写入RFID：" + factory_num + "," + product_num + "," + textBox_serial_NO.Text + "," + product_date + "," + sn[0];
                        logHandleUtils.writeLog(log);
                        serial_num_int++;
                        textBox_serial_NO.Text = serial_num_int.ToString();
                        string config = factory_num + "-" + product_num + "-" + textBox_serial_NO.Text + "-" + Check_WorkCommand.IsChecked.ToString() + "-" + work_command_num + "-";
                        configHandleUtils.writeConfig(config);
                        RfidInfo info = new RfidInfo() { InfoType = RFIDInfoEnum.WriteSuccess, Content = $"写入RFID成功{factory_num},{ product_num },{textBox_serial_NO.Text},{product_date},{sn[0]}" };
                        SendRfidOperateEvent(info);
                    }
                    else
                    {
                        soundPlay.playFail();
                        textBox_info_box.Background = new SolidColorBrush(Constans.errorColor);
                        textBox_info_box.Text = dataUtils.codeAnalyse(code);
                        RfidInfo info = new RfidInfo() { InfoType = RFIDInfoEnum.WriteError, Content = $"写入RFID失败{factory_num},{ product_num },{textBox_serial_NO.Text},{product_date}" };
                        SendRfidOperateEvent(info);
                    }
                }
            }
            else
            {
                soundPlay.playFail();
                textBox_info_box.Background = new SolidColorBrush(Constans.errorColor);
                textBox_info_box.Text = "请选择串口。";
            }

            logHandleUtils.writeLog(textBox_info_box.Text);
        }
        /// <summary>
        /// 烧录信息
        /// </summary>
        /// <returns></returns>
        public bool burn_message(out string OperateMessage)
        {
            OperateMessage = string.Empty;
            if (serialPort != null && serialPort.IsOpen)
            {
                if (true)
                {
                    string result = burnFilterData(factory_num, product_num, product_date, textBox_serial_NO.Text);
                    int code = dataUtils.getResultCode(recieveData);
                    //Thread.Sleep(200);
                    if (code == 80)
                    {
                        soundPlay.playSuccess();
                        serial_num_int = int.Parse(serial_num);
                        result = getFilterSN();
                        string[] sn = dataUtils.getRecieveSN(recieveData);
                        //textBox_info_box.Text = result + "\n" + "sn:" + sn[0] + "\n" + "滤芯使用时长(秒):" + sn[1];
                        OperateMessage = "烧写成功！\n" + "sn:" + sn[0] + "\n" + "滤芯使用时长(秒):" + sn[1];

                        textBox_info_box.Background = new SolidColorBrush(Constans.successColor);
                        string data = factory_num + "\t" + product_num + "\t" + textBox_serial_NO.Text + "\t" + product_date + "\t" + sn[0];
                        if (Check_WorkCommand.IsChecked == true)
                        {
                            data += "\t" + work_command_num;
                        }
                        dataHandleUtils.writeData(data);

                        log = "写入RFID：" + factory_num + "," + product_num + "," + textBox_serial_NO.Text + "," + product_date + "," + sn[0];
                        logHandleUtils.writeLog(log);
                        serial_num_int++;
                        textBox_serial_NO.Text = serial_num_int.ToString();
                        string config = factory_num + "-" + product_num + "-" + textBox_serial_NO.Text + "-" + Check_WorkCommand.IsChecked.ToString() + "-" + work_command_num + "-";
                        configHandleUtils.writeConfig(config);
                    }
                    else
                    {
                        soundPlay.playFail();
                        textBox_info_box.Background = new SolidColorBrush(Constans.errorColor);
                        textBox_info_box.Text = dataUtils.codeAnalyse(code);
                    }
                }
            }
            else
            {
                soundPlay.playFail();
                textBox_info_box.Background = new SolidColorBrush(Constans.errorColor);
                OperateMessage = "请打开串口。";
            }

            logHandleUtils.writeLog(textBox_info_box.Text);
            return false;
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            string logPath = logHandleUtils.getLogPath();

            logWindow logwindow = new logWindow();

            logwindow.Show();
            try
            {
                logwindow.textBox_log_window.Text = File.ReadAllText(logPath, Encoding.Default);
            }
            catch (Exception ex)
            {
                textBox_info_box.Text = ex.Message;
            }

        }

        private void textBox_factory_NO_GotFocus(object sender, RoutedEventArgs e)
        {
            if (textBox_factory_NO.Text == "输入两位工厂编号")
            {
                textBox_factory_NO.Text = "";
            }
        }

        private void textBox_product_NO_GotFocus(object sender, RoutedEventArgs e)
        {
            if (textBox_product_NO.Text == "输入滤芯产品编号")
            {
                textBox_product_NO.Text = "";
            }
        }

        private void textBox_serial_NO_GotFocus(object sender, RoutedEventArgs e)
        {
            if (textBox_serial_NO.Text == "输入起始流水号")
            {
                textBox_serial_NO.Text = "";
            }
        }

        private void textBox_product_date_GotFocus(object sender, RoutedEventArgs e)
        {
            if (textBox_product_date.Text == "输入6位生产日期")
            {
                textBox_product_date.Text = "";
            }
        }

        private void textBox_serial_NO_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            serial_num = textBox_serial_NO.Text;
        }

        private string getHeartBeat()
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                try
                {
                    serialPort.Write(Constans.getHeartBeatCode, 0, Constans.getHeartBeatCode.Length);
                    int sleep = 0;
                    while (sleep <= sleepTime * tryTimes)
                    {
                        if (serialPort.BytesToRead == 4)
                        {
                            recieveData = getReturnComString();
                            sleep = sleepTime * tryTimes + 1;
                        }
                        else
                        {
                            Thread.Sleep(sleepTime);
                            sleep = sleep + sleepTime;
                        }
                    }
                    logHandleUtils.writeLog("读取工装心跳包。");
                }
                catch (Exception ex)
                {
                    textBox_info_box.Text = ex.Message;
                }
                Thread.Sleep(500);
                return dataUtils.resultAnalyse(recieveData);
            }
            else
            {
                return "请选择串口。";
            }
        }
        private string getFilterData()
        {
            if (serialPort != null && serialPort.IsOpen)
            {

                try
                {
                    serialPort.Write(Constans.getFilterDataCode, 0, Constans.getFilterDataCode.Length);
                    int sleep = 0;
                    while (sleep <= sleepTime * tryTimes)
                    {
                        if (serialPort.BytesToRead == 4 || serialPort.BytesToRead == 20)
                        {
                            recieveData = getReturnComString();
                            sleep = sleepTime * tryTimes + 1;
                        }
                        else
                        {
                            Thread.Sleep(sleepTime);
                            sleep = sleep + sleepTime;
                        }
                    }



                    logHandleUtils.writeLog("读取滤芯数据。");
                }
                catch (Exception ex)
                {
                    textBox_info_box.Text = ex.Message;
                }
                return dataUtils.resultAnalyse(recieveData);

                //else
                //{
                //    return "数据读取异常。";
                //}
            }
            else
            {
                return "请选择串口。";
            }

        }
        public string getFilterSN()
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                try
                {
                    serialPort.Write(Constans.getFilterSNCode, 0, Constans.getFilterSNCode.Length);
                    int sleep = 0;
                    while (sleep <= sleepTime * tryTimes)
                    {
                        if (serialPort.BytesToRead == 4 || serialPort.BytesToRead == 15)
                        {
                            recieveData = getReturnComString();
                            sleep = sleepTime * tryTimes + 1;
                        }
                        else
                        {
                            Thread.Sleep(sleepTime);
                            sleep = sleep + sleepTime;
                        }
                    }

                    logHandleUtils.writeLog("读取SN及滤芯使用时长");
                }
                catch (Exception ex)
                {
                    textBox_info_box.Text = ex.Message;
                }
                return dataUtils.resultAnalyse(recieveData);
            }
            else
            {
                return "请选择串口。";
            }
        }

        public void getSN()
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                try
                {
                    serialPort.Write(Constans.getFilterSNCode, 0, Constans.getFilterSNCode.Length);
                    int sleep = 0;
                    while (sleep <= sleepTime * tryTimes)
                    {
                        if (serialPort.BytesToRead == 4 || serialPort.BytesToRead == 20)
                        {
                            recieveData = getReturnComString();
                            sleep = sleepTime * tryTimes + 1;
                        }
                        else
                        {
                            Thread.Sleep(sleepTime);
                            sleep = sleep + sleepTime;
                        }
                    }

                    logHandleUtils.writeLog("读取SN及滤芯使用时长");

                }
                catch (Exception ex)
                {
                    textBox_info_box.Text = ex.Message;
                }
            }
            else
            {
                textBox_info_box.Background = new SolidColorBrush(Constans.errorColor);
                soundPlay.playFail();
                textBox_info_box.Text = "请选择串口。";
            }
        }

        //单次读取RFID
        private void button_read_filter_data_Click(object sender, RoutedEventArgs e)
        {
            recieveData = "";
            string showResult = DateTime.Now.ToString();
            string result = getFilterData();

            SerialPortHandle(true);//打开串口
            
            RfidInfo info = new RfidInfo() { InfoType = RFIDInfoEnum.LogInfo, Content = $"开始进行RFID信息读取" };
            SendRfidOperateEvent(info);

            string[] data = dataUtils.getRecieveFilterData(recieveData);//data为结果.data[0]:工厂编号,data[1]:产品编号,data[2]:生产日期,data[3]:流水号
            int filterCode = dataUtils.getResultCode(recieveData);


            if (filterCode == 84)
            {
                showResult = result + "\n" + dataUtils.showFilterData(data);
            }
            else
            {
                showResult = result;//错误在此显示,data里面全为空
            }

            result = getFilterSN();
            string[] sn = dataUtils.getRecieveSN(recieveData);//sn[0]为sn序列号，sn[1]为滤芯使用时长
            int snCode = dataUtils.getResultCode(recieveData);

            if (snCode == 85)
            {
                showResult += "\n" + "sn:" + sn[0] + "\n" + "滤芯使用时长(秒):" + sn[1];
            }
            else
            {
                showResult = result;//错误走此分支
            }

            if ((filterCode == 84) && (snCode == 85))
            {
                textBox_info_box.Background = new SolidColorBrush(Constans.successColor);
                soundPlay.playSuccess();

                if (lastWriteInfo != sn[0])
                {
                    logHandleUtils.writeLog($"读取到的为：{sn[0]}，上一个为：{lastWriteInfo}，比对失败");
                    info = new RfidInfo() { InfoType = RFIDInfoEnum.ReadError, Content = $"读取到的为：{sn[0]}，上一个为：{lastWriteInfo}，比对失败" };
                    SendRfidOperateEvent(info);
                }
                else
                {
                    info = new RfidInfo() { InfoType = RFIDInfoEnum.ReadSuccess, Content = $"读取到的为：{sn[0]}，上一个为：{lastWriteInfo}，比对成功" };
                    SendRfidOperateEvent(info);
                }
            }
            else
            {
                textBox_info_box.Background = new SolidColorBrush(Constans.errorColor);
                soundPlay.playFail();

                info = new RfidInfo() { InfoType = RFIDInfoEnum.ReadError, Content = $"读取到的为：{sn[0]}，上一个为：{lastWriteInfo}，比对失败" };
                SendRfidOperateEvent(info);
            }

            textBox_info_box.Text = showResult;

            logHandleUtils.writeLog(textBox_info_box.Text);

        }
        /// <summary>
        /// 连续读取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void button_read_successive_Click(object sender, RoutedEventArgs e)
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                if (bReadSuccessiveFlag == false)
                {
                    bReadSuccessiveFlag = true;
                    button_read_filter_data_successive.Content = "连续读取中...";
                    button_read_filter_data.IsEnabled = false;
                    button_check_tool.IsEnabled = false;
                    button_view_log.IsEnabled = false;
                    button_burned.IsEnabled = false;
                    button_burn.IsEnabled = false;
                    button_reburn_data.IsEnabled = false;
                    readTimer.Start();
                }
                else
                {
                    bReadSuccessiveFlag = false;
                    button_read_filter_data_successive.Content = "连续读取";
                    button_read_filter_data.IsEnabled = true;
                    button_check_tool.IsEnabled = true;
                    button_view_log.IsEnabled = true;
                    button_burned.IsEnabled = true;
                    button_burn.IsEnabled = true;
                    button_reburn_data.IsEnabled = true;
                    readTimer.Stop();
                }
            }
            else if (bReadSuccessiveFlag == false)
            {
                textBox_info_box.Background = new SolidColorBrush(Constans.errorColor);
                soundPlay.playFail();
                textBox_info_box.Text = "请选择串口。";
            }
        }

        private void button_check_tool_Click(object sender, RoutedEventArgs e)
        {
            textBox_info_box.Text = getHeartBeat();
        }

        private string getReturnComString()
        {
            char hexH;
            char hexL;
            byte receivebyte;
            string dataToshow = "";
            byte[] array = { };
            ArrayList byteList = new ArrayList();
            while (serialPort.BytesToRead > 0)

            {

                receivebyte = (byte)serialPort.ReadByte();
                byteList.Add(receivebyte);
                hexH = HexChar[receivebyte / 16];
                hexL = HexChar[receivebyte % 16];
                dataToshow += hexH.ToString() + hexL.ToString();
            }

            return dataToshow;
        }

        private byte[] getReturnComBytes()
        {
            byte receivebyte;
            byte[] array = { };
            ArrayList byteList = new ArrayList();
            try
            {
                while (serialPort.BytesToRead > 0)
                {

                    receivebyte = (byte)serialPort.ReadByte();
                    byteList.Add(receivebyte);

                }
            }
            catch (Exception ex)
            {
                textBox_info_box.Text = "读取异常。" + ex.Message;
            }

            byte[] bytes = dataUtils.arrayToBytes(byteList);
            return bytes;
        }

        private string burnFilterData(string f_num, string p_num, string p_date, string s_num)
        {
            byte[] f_num_b = new byte[4];
            f_num_b = dataUtils.f_NumToHex(f_num);
            byte[] p_num_b = new byte[4];
            p_num_b = dataUtils.f_NumToHex(p_num);
            byte[] p_date_b = new byte[4];
            p_date_b = dataUtils.s_NumToHex(p_date);
            byte[] s_num_b = new byte[4];
            s_num_b = dataUtils.s_NumToHex(s_num);

            ArrayList f_num_l = dataUtils.bytesToArray(f_num_b);
            ArrayList p_num_l = dataUtils.bytesToArray(p_num_b);
            ArrayList p_date_l = dataUtils.bytesToArray(p_date_b);
            ArrayList s_num_l = dataUtils.bytesToArray(s_num_b);

            ArrayList comandsList = new ArrayList();

            comandsList.AddRange(Constans.burnFilterDataCode);
            comandsList.AddRange(f_num_l);
            comandsList.AddRange(p_num_l);
            comandsList.AddRange(p_date_l);
            comandsList.AddRange(s_num_l);

            byte xor = (byte)comandsList[0];
            for (int i = 1; i < comandsList.Count; i++)
            {
                xor ^= (byte)comandsList[i];
            }
            comandsList.Add(xor);

            byte[] comandsBytes = dataUtils.arrayToBytes(comandsList);

            string str = "";
            foreach (byte b in comandsBytes)
            {
                str += b.ToString() + " ";
            }
            try
            {
                serialPort.Write(comandsBytes, 0, comandsBytes.Length);
                int sleep = 0;
                recieveData = getReturnComString();
                while (sleep <= sleepTime * tryTimes)
                {
                    if (serialPort.BytesToRead == 4 || serialPort.BytesToRead == 11)
                    {

                        recieveData = getReturnComString();
                        sleep = sleepTime * tryTimes + 1;
                    }

                    else
                    {
                        Thread.Sleep(sleepTime);
                        sleep = sleep + sleepTime;
                    }
                }


                logHandleUtils.writeLog("写入数据：" + str);
            }

            catch (Exception ex)
            {
                textBox_info_box.Text = "写入RFID数据出错。" + ex.Message;
                logHandleUtils.writeLog("写入RFID数据出错。" + ex.Message);
            }
            Thread.Sleep(500);
            return dataUtils.resultAnalyse(recieveData);
        }

        private string reburnFilterData(string f_num, string p_num, string p_date, string s_num)
        {
            recieveData = "";
            byte[] f_num_b = new byte[4];
            f_num_b = dataUtils.f_NumToHex(f_num);
            byte[] p_num_b = new byte[4];
            p_num_b = dataUtils.f_NumToHex(p_num);
            byte[] p_date_b = new byte[4];
            p_date_b = dataUtils.s_NumToHex(p_date);
            byte[] s_num_b = new byte[4];
            s_num_b = dataUtils.s_NumToHex(s_num);

            ArrayList f_num_l = dataUtils.bytesToArray(f_num_b);
            ArrayList p_num_l = dataUtils.bytesToArray(p_num_b);
            ArrayList p_date_l = dataUtils.bytesToArray(p_date_b);
            ArrayList s_num_l = dataUtils.bytesToArray(s_num_b);


            ArrayList comandsList = new ArrayList();
            byte b1 = 0xfa;
            byte b2 = 17;
            byte b3 = 05;

            comandsList.Add(b1);
            comandsList.Add(b2);
            comandsList.Add(b3);
            comandsList.AddRange(f_num_l);
            comandsList.AddRange(p_num_l);
            comandsList.AddRange(p_date_l);
            comandsList.AddRange(s_num_l);

            byte xor = (byte)comandsList[0];
            for (int i = 1; i < comandsList.Count; i++)
            {
                xor ^= (byte)comandsList[i];
            }
            comandsList.Add(xor);

            byte[] comandsBytes = dataUtils.arrayToBytes(comandsList);
            string str = "";
            foreach (byte b in comandsBytes)
            {
                str += b.ToString() + " ";
            }
            recieveData = "";
            try
            {
                serialPort.Write(comandsBytes, 0, comandsBytes.Length);
                int sleep = 0;
                while (sleep <= sleepTime * tryTimes)
                {
                    if (serialPort.BytesToRead == 4 || serialPort.BytesToRead == 11)
                    {
                        recieveData = getReturnComString();
                        sleep = sleepTime * tryTimes + 1;
                    }
                    else
                    {
                        Thread.Sleep(sleepTime);
                        sleep = sleep + sleepTime;
                    }
                }

                logHandleUtils.writeLog("重写入数据：" + str);
            }
            catch (Exception ex)
            {
                textBox_info_box.Text = "写入RFID数据出错。" + ex.Message;
                logHandleUtils.writeLog("写入RFID数据出错。" + ex.Message);
            }

            Thread.Sleep(500);
            return dataUtils.resultAnalyse(recieveData);

        }
        //重新烧录
        public void button_reburn_Click(object sender, RoutedEventArgs e)
        {
            RfidInfo info;
            lastWriteInfo = string.Empty;
            recieveData = "";
            textBox_info_box.Background = new SolidColorBrush(Constans.whtieColor);

            SerialPortHandle(true);//打开串口
            if (serialPort != null && serialPort.IsOpen)
            {
                info = new RfidInfo() { InfoType = RFIDInfoEnum.LogInfo, Content = $"串口为打开状态，开始准备RFID信息写入" };
                SendRfidOperateEvent(info);
                if (Regex.IsMatch(textBox_serial_NO.Text, @"^[+-]?\d*[.]?\d*$"))
                {
                    string result = reburnFilterData(factory_num, product_num, product_date, textBox_serial_NO.Text);
                    int code = dataUtils.getResultCode(recieveData);

                    if (code == 80)
                    {
                        soundPlay.playSuccess();
                        serial_num_int = int.Parse(serial_num);
                        result = getFilterSN();
                        string[] sn = dataUtils.getRecieveSN(recieveData);
                        //textBox_info_box.Text = result + "\n" + "sn:" + sn[0] + "\n" + "滤芯使用时长(秒):" + sn[1];
                        textBox_info_box.Text = "烧写成功！\n" + "sn:" + sn[0] + "\n" + "滤芯使用时长(秒):" + sn[1];

                        textBox_info_box.Background = new SolidColorBrush(Constans.successColor);
                        string data = factory_num + "\t" + product_num + "\t" + textBox_serial_NO.Text + "\t" + product_date + "\t" + sn[0];

                        if (Check_WorkCommand.IsChecked == true)
                        {
                            data += "\t" + work_command_num;
                        }

                        dataHandleUtils.writeData(data);
                        log = "重新写入RFID：" + factory_num + "," + product_num + "," + textBox_serial_NO.Text + "," + product_date + "," + sn[0];
                        logHandleUtils.writeLog(log);
                        serial_num_int++;
                        textBox_serial_NO.Text = serial_num_int.ToString();
                        string config = factory_num + "-" + product_num + "-" + textBox_serial_NO.Text + "-" + Check_WorkCommand.IsChecked.ToString() + "-" + work_command_num + "-";
                        configHandleUtils.writeConfig(config);
                        lastWriteInfo = sn[0];
                        button_read_filter_data_Click(null, null);//写入完毕后直接单次读取
                    }
                    else
                    {
                        soundPlay.playFail();
                        textBox_info_box.Background = new SolidColorBrush(Constans.errorColor);
                        textBox_info_box.Text = dataUtils.codeAnalyse(code);

                        info = new RfidInfo() { InfoType = RFIDInfoEnum.WriteError, Content = $"RFID信息烧录失败，code={code}" };
                        SendRfidOperateEvent(info);
                    }
                }
                else
                {
                    textBox_serial_NO.Background = new SolidColorBrush(Constans.successColor);
                    textBox_info_box.Text = "流水号需为数字。";

                    info = new RfidInfo() { InfoType = RFIDInfoEnum.WriteError, Content = $"RFID信息烧录失败，流水号需为数字" };
                    SendRfidOperateEvent(info);
                }
            }
            else
            {
                textBox_info_box.Background = new SolidColorBrush(Constans.errorColor);
                soundPlay.playFail();
                textBox_info_box.Text = "请选择串口。";

                info = new RfidInfo() { InfoType = RFIDInfoEnum.WriteError, Content = $"RFID信息烧录失败，串口未打开" };
                SendRfidOperateEvent(info);
            }
            //SerialPortHandle(false);//关闭串口
            logHandleUtils.writeLog(textBox_info_box.Text);

        }

        private bool checkRfidExisted()
        {
            getSN();

            int code = dataUtils.getResultCode(recieveData);
            if (code == 85)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void button_performance_test_Click(object sender, RoutedEventArgs e)
        {
            factory_num = "01";
            product_num = "a0";
            product_date = "160924";
            serial_num_int = 1;

            int success_count = 0;
            int failed_count = 0;

            while (true) {

                string title = "=====================NO." + serial_num_int + " ===================== ";
                logHandleUtils.writeLog(title);
                string result = reburnFilterData(factory_num, product_num, product_date, serial_num_int.ToString());
                string data = factory_num + " " + product_num + " " + product_date + " " + serial_num_int;
                log = "重新写入RFID：" + data;
                logHandleUtils.writeLog(log);

                int code = dataUtils.getResultCode(recieveData);
                logHandleUtils.writeLog(code.ToString());

                if (code == 80)
                {
                    success_count++;

                }
                else if (code == 0)
                {
                    failed_count++;
                    logHandleUtils.writeLog("resutl:" + result);
                    logHandleUtils.writeLog(recieveData);
                }
                else
                {
                    failed_count++;
                }
                logHandleUtils.writeLog(success_count.ToString() + " " + failed_count.ToString());
                serial_num_int++;
                Thread.Sleep(2000);
            }
        }

        private void button_burned_Click(object sender, RoutedEventArgs e)
        {
            string dataPath = dataHandleUtils.getDataFilePath();
            System.Diagnostics.Process.Start("Explorer.exe", dataHandleUtils.getDataFolderPath());
            DataWindow datawindow = new DataWindow();

            datawindow.Show();
            try
            {
                datawindow.textBox_data_window.Text = File.ReadAllText(dataPath, Encoding.Default);
            }
            catch (Exception ex)
            {
                textBox_info_box.Text = ex.Message;
            }
        }

        private void Check_WorkCommand_Checked(object sender, RoutedEventArgs e)
        {
            textBox_work_command_NO.IsEnabled = true;
        }

        private void Check_WorkCommand_Unchecked(object sender, RoutedEventArgs e)
        {
            textBox_work_command_NO.IsEnabled = false;
        }

        /// <summary>
        /// RFID读取事件
        /// </summary>
        public delegate void RFIDInfoDelegate(RfidInfo info);

        /// <summary>
        /// 
        /// </summary>
        public event RFIDInfoDelegate RFIDInfoEvent;

        protected virtual void SendRfidOperateEvent(RfidInfo info)
        {
            if (this.RFIDInfoEvent != null)
            {
                this.RFIDInfoEvent(info);
            }
        }
        public void SerialPortHandle(bool IsOpen)
        {
            try
            {
                if (serialPort == null)
                {
                    serialPort = serialPortUtils.OpenPort(textBoxCom.Text.Trim(), logHandleUtils);
                }
                if (serialPort != null)
                {
                    if (IsOpen)
                    {
                        if (!serialPort.IsOpen)
                        {
                            serialPort.Open();
                        }
                    }
                    else if (serialPort.IsOpen)
                    {
                        serialPort.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                RfidInfo info = new RfidInfo() { InfoType = RFIDInfoEnum.PortError, Content = $"RFID串口操作失败{ex.Message}" };
                SendRfidOperateEvent(info);
            }
        }

    }
    /// <summary>
    /// RFID消息
    /// </summary>
    public class RfidInfo
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public RFIDInfoEnum InfoType { set; get; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Content { set; get; }
    }
    /// <summary>
    /// RFID消息类型枚举
    /// </summary>
    public enum RFIDInfoEnum
    {
        /// <summary>
        /// 端口错误
        /// </summary>
        PortError,
        /// <summary>
        /// 写入错误
        /// </summary>
        WriteError,
        /// <summary>
        /// 写入成功
        /// </summary>
        WriteSuccess,
        /// <summary>
        /// 读取错误
        /// </summary>
        ReadError,
        /// <summary>
        /// 读取成功消息
        /// </summary>
        ReadSuccess,
        /// <summary>
        /// 记录型消息
        /// </summary>
        LogInfo
    }
}
