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
        bool DataPreprocessing(string filepath, Program program, Recipe recipe, TestRecord record, int startIndex, uint options);
        //string EventDescriptor(string filepath, Program program, Recipe recipe, TestRecord record, string info);
        uint LoadRawToSource(string testFilePath, ref SourceData sd);
    }
}