using System;
using System.Collections.ObjectModel;

namespace BCLabManager.Model
{
    public interface ITesterProcesser
    {
        DateTime[] GetTimeFromRawData(ObservableCollection<string> fileList);
        bool CheckChannelNumber(string filepath, string channelnumber);
    }
}