using System;
using System.IO;
using System.Text;

namespace RFIDLib
{
    public class DataHandleUtils
    {
        static FileStream logfile;
        static string logPath;
        static string logFolder;

        public void writeData(string data)
        {
            string nowTime = DateTime.Now.ToString();

                logfile = new FileStream(logPath, FileMode.Append);
                StreamWriter sw = new StreamWriter(logfile, Encoding.Default);
                sw.WriteLine(data);
                sw.Close();
                logfile.Close();

        }
        public void createDataPath()
        {
            string nowTime = DateTime.Now.ToString("yyyy-MM-dd-HH-mm");
            logFolder = Environment.CurrentDirectory + "\\烧写记录\\";
            logPath = logFolder + nowTime + ".xls";
            if (!Directory.Exists(logFolder))
            {
                Directory.CreateDirectory(logFolder);
            }
            if (!File.Exists(logPath))
            {
                logfile = new FileStream(logPath, FileMode.Create);
                StreamWriter sw = new StreamWriter(logfile, Encoding.Default);
                string title = "工厂编号" + "\t" + "产品编号" + "\t" + "流水号" + "\t" + "生产日期" + "\t" + "SN号" + "\t" + "工作令编码";
                sw.WriteLine(title);
                sw.Close();
                logfile.Close();
            }
        }

        public string getDataFolderPath()
        {
            return logFolder;
        }

        public string getDataFilePath()
        {
            return logPath;
        }
    }




}