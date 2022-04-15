using System;
using System.Collections.ObjectModel;

namespace BCLabManager.Model
{
    public static class TesterProcesserOptions
    {
        public static readonly uint SkipTemperatureCheck = 0x0001;
    }
    public interface ITesterProcesser
    {
        DateTime[] GetTimeFromRawData(ObservableCollection<string> fileList);
        bool CheckChannelNumber(string filepath, string channelnumber);
        string GetChannelName(string filepath);
        bool CheckFileFormat(string filepath);

        //fix errors in filepath, check steps, check continuity, check boundaries, create standard format file
        bool DataPreprocessing(string filepath, Program program, Recipe recipe, TestRecord record, int startIndex, uint options);
        StandardRow ConvertToStdRow(uint index, string rawRow);
        double GetDischargeCapacityFromRawData(ObservableCollection<string> fileList);
    }
}