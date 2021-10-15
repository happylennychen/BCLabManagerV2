using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager
{
    static public class RunningLog
    {
        //static private StreamWriter sw;
        //static void Init()
        //{
        //}
        static public void Write(string log)
        {
            FileStream fs = new FileStream(GlobalSettings.RunningLogFilePath, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(log);
            sw.Flush();
            sw.Close();
            fs.Close();
        }
        static public void NewLog(string logName = "", int index = 0)
        {
            string newLogName;
            if (index != 0)
                newLogName = logName + "-" + index;
            else
                newLogName = logName;
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Running Log\\", $"{newLogName}.log");
            if (File.Exists(path))
                NewLog(logName, index + 1);
            else
                GlobalSettings.RunningLogFilePath = path;
        }
    }
}
