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
    public class PseudoProcesser : ITesterProcesser
    {
        public DateTime[] GetTimeFromRawData(ObservableCollection<string> fileList)
        {
            return null;
        }
        public bool CheckChannelNumber(string filepath, string channelnumber)
        {
            return true;
        }

        public string GetChannelName(string filepath)
        {
            string output = "Ch";
            return output;
        }

        public bool CheckFileFormat(string filepath)
        {
            return true;
        }

        public bool DataPreprocessing(string filepath, Program program, Recipe recipe, TestRecord record, int startIndex, uint options)
        {
            return true;
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
