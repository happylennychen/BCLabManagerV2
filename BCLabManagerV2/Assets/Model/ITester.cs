using System;
using System.Collections.ObjectModel;

namespace BCLabManager.Model
{
    internal interface ITester
    {
        DateTime[] GetTimeFromRawData(ObservableCollection<string> fileList);
    }
}