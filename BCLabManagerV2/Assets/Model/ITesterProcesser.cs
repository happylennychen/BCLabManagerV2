using System;
using System.Collections.ObjectModel;

namespace BCLabManager.Model
{
    public interface ITesterProcesser
    {
        DateTime[] GetTimeFromRawData(ObservableCollection<string> fileList);
        bool CheckChannelNumber(string filepath, string channelnumber);
        string GetChannelName(string filepath);
        bool CheckFileFormat(string filepath);
        bool DataPreprocessing(string filepath, Program program, Recipe recipe, TestRecord record);
    }
}