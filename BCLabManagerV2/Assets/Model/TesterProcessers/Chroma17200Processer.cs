using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class Chroma17200Processer : ITesterProcesser
    {
        public DateTime[] GetTimeFromRawData(ObservableCollection<string> fileList)
        {
            DateTime[] output = new DateTime[2];
            List<DateTime> StartTimes = new List<DateTime>();
            List<DateTime> EndTimes = new List<DateTime>();
            foreach (var fn in fileList)
            {
                DateTime[] timepair = GetTimesFromFile(fn);
                StartTimes.Add(timepair[0]);
                EndTimes.Add(timepair[1]);
            }
            output[0] = GetEarliest(StartTimes);
            output[1] = GetLatest(EndTimes);
            return output;
        }

        private DateTime[] GetTimesFromFile(string fn)
        {
            DateTime[] output = new DateTime[2];
            FileStream fs = new FileStream(fn, FileMode.Open);
            StreamReader sw = new StreamReader(fs);
            sw.ReadLine();
            sw.ReadLine();
            string startTimeLine = sw.ReadLine();
            string startTimeStr = startTimeLine.Substring(17, 19);
            output[0] = DateTime.Parse(startTimeStr);
            string endTimeLine = sw.ReadLine();
            string endTimeStr = endTimeLine.Substring(15, 19);
            output[1] = DateTime.Parse(endTimeStr);
            sw.Close();
            fs.Close();
            return output;
        }

        private DateTime GetEarliest(List<DateTime> startTimes)
        {
            return startTimes.Min();
        }

        private DateTime GetLatest(List<DateTime> endTimes)
        {
            return endTimes.Max();
        }

        public bool CheckChannelNumber(string filepath, string channelnumber)
        {
            return GetChannelName(filepath) == channelnumber;
        }

        private string GetChannelName(string filepath)
        {
            string output = "Ch ";
            FileStream fs = new FileStream(filepath, FileMode.Open);
            StreamReader sw = new StreamReader(fs);
            sw.ReadLine();
            string channelNumberLine = sw.ReadLine();
            string channelNumberStr = channelNumberLine.Substring(15, 1);
            output += channelNumberStr;
            sw.Close();
            fs.Close();
            return output;
        }
    }
}
