using System;
using System.IO;
using System.Text;

namespace RFIDLib
{
    public class ConfigHandleUtils
    {
        static FileStream logfile;
        static string logPath;

        public void writeConfig(string data)
        {
            string nowTime = DateTime.Now.ToString();

                logfile = new FileStream(logPath, FileMode.Truncate);
                StreamWriter sw = new StreamWriter(logfile, Encoding.Default);
                sw.WriteLine(data);
                sw.Close();
                logfile.Close();

        }
        public void createConfigPath()
        {
            string logFolder = Environment.CurrentDirectory;
            logPath = logFolder + "user.config";

            if (!File.Exists(logPath))
            {
                logfile = new FileStream(logPath, FileMode.Create);
                StreamWriter sw = new StreamWriter(logfile);
                sw.Close();
                logfile.Close();
            }
        }

        public string getConfigFilePath()
        {
            return logPath;
        }
    }




}