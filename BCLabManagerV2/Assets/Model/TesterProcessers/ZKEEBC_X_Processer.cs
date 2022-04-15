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
    public class ZKEEBC_X_Processer : ITesterProcesser
    {
        public DateTime[] GetTimeFromRawData(ObservableCollection<string> fileList)
        {
            return null;
        }
        public bool CheckChannelNumber(string filepath, string channelnumber)
        {
            return GetChannelName(filepath) == channelnumber;
        }

        public string GetChannelName(string filepath)
        {
            string output = "Ch";
            FileStream fs = new FileStream(filepath, FileMode.Open);
            StreamReader sw = new StreamReader(fs);
            sw.ReadLine();
            string channelNumberLine = sw.ReadLine();
            string channelNumberStr = channelNumberLine.Substring(6, 1);
            output += channelNumberStr;
            sw.Close();
            fs.Close();
            return output;
        }

        public bool CheckFileFormat(string filepath)
        {
            if (Path.GetExtension(filepath) != ".dat")
                return false;

            FileStream fs = new FileStream(filepath, FileMode.Open);
            StreamReader sw = new StreamReader(fs);
            //for (int i = 0; i < 3; i++)
            //    sw.ReadLine();
            string urlLine = sw.ReadLine();
            if (!urlLine.Contains("EBC-X0510"))
            {
                sw.Close();
                fs.Close();
                return false;
            }

            sw.Close();
            fs.Close();
            return true;
        }

        public bool DataPreprocessing(string filepath, Program program, Recipe recipe, TestRecord record, int startIndex, uint options)
        {
            return true;
        }

        public string EventDescriptor(string filepath, Program program, Recipe recipe, TestRecord record, string info)
        {
            throw new NotImplementedException();
        }

        public uint LoadRawToSource(string testFilePath, ref SourceData sd)
        {
            return ErrorCode.UNDEFINED;
        }

        public StandardRow ConvertToStdRow(uint index, string rawRow)
        {
            return null;
        }

        public double GetDischargeCapacityFromRawData(ObservableCollection<string> fileList)
        {
            return 0.0;
        }
    }
}
