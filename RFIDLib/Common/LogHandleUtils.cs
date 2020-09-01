using System;
using System.IO;
using System.Text;

namespace RFIDLib
{
    public class LogHandleUtils
    {
        static FileStream logfile;
        static string logPath;
        public LogHandleUtils()
        {


        }

        public void writeLog(string log)
        {
            string nowTime = DateTime.Now.ToString();

   
                logfile = new FileStream(logPath, FileMode.Append);
                StreamWriter sw = new StreamWriter(logfile, Encoding.Default);
                sw.WriteLine(nowTime + " - " + log);
                sw.Close();
                logfile.Close();
            

        }

        public void createLogPath()
        {
            string nowTime = DateTime.Now.ToString("yyyy-MM-dd-HH-mm");
            string logFolder = Environment.CurrentDirectory + "\\log\\";
            logPath = logFolder + nowTime + ".log";
            if (!Directory.Exists(logFolder))
            {
                Directory.CreateDirectory(logFolder);
            }
            if(!File.Exists(logPath))
            {
                logfile = new FileStream(logPath, FileMode.Create);

                logfile.Close();
            }
            
        }

        public string getLogPath()
        {
            return logPath;
        }

    }
}